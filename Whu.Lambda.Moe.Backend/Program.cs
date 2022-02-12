global using Microsoft.EntityFrameworkCore;

using AspNet.Security.OAuth.GitHub;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

using System.Security.Claims;
using System.Security.Principal;

using Whu.Lambda.Moe.Backend;
using Whu.Lambda.Moe.Backend.Dao;
using Whu.Lambda.Moe.Backend.GrpcServices;
using Whu.Lambda.Moe.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

var serviceBuilder = builder.Services;
IConfiguration configuration = builder.Configuration;

#region ConfigureServices
serviceBuilder.AddGrpc();
serviceBuilder
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(option =>
    {
        option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        option.ExpireTimeSpan = TimeSpan.FromDays(14);
        option.SlidingExpiration = true;
    })
    .AddGitHub(options =>
    {
        options.ClientId = configuration["Github:ClientID"];
        options.ClientSecret = configuration["Github:ClientSecret"];
        options.CallbackPath = "/callback/auth/GitHub";
        options.AccessDeniedPath = "/";
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = configuration["Microsoft:ClientID"];
        options.ClientSecret = configuration["Microsoft:ClientSecret"];
        options.CallbackPath = "/callback/auth/Microsoft";
        options.AccessDeniedPath = "/";
    });
serviceBuilder
    .AddAuthorization(options =>
    {
        var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddRequirements(new AuthService.AuthRequirement())
            .RequireRole(AuthService.UserRole)
            .Build();
        options.DefaultPolicy = policy;
    })
    .AddRouting()
    .AddMemoryCache()
    .AddSingleton<IAuthorizationHandler, AuthService>()
    .AddDbContextPool<DbService>(option => option.UseSqlite(configuration.GetConnectionString("sqlite")));
#endregion

var app = builder.Build();

static void MapAuthenticaters(IEndpointRouteBuilder endpoint, IEnumerable<string> schemes)
{
    var cacheOptions = new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromDays(14) };
    var authPropertis = new AuthenticationProperties()
    {
        AllowRefresh = true,
        IsPersistent = true
    };
    foreach (string scheme in schemes)
    {
        endpoint.MapGet($"/login/{scheme}", async context =>
        {
            var res = await context.AuthenticateAsync();
            if (res.Principal?.Identity?.Name is string token)
            {
                var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
                if (cache.TryGetValue(token, out _))
                {
                    context.Response.Redirect("/");
                    return;
                }
            }

            res = await context.AuthenticateAsync(scheme);
            var principal = res.Principal;
            string? oauthId = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (res.Succeeded && principal != null && oauthId != null)
            {
                var db = context.RequestServices.GetRequiredService<DbService>();
                var oauthTask = db.OAuths.FirstOrDefaultAsync(oauth => oauth.OAuthId == oauthId && oauth.Scheme == scheme);

                token = Guid.NewGuid().ToString();
                var cookie = new GenericPrincipal(new GenericIdentity(token, CookieAuthenticationDefaults.AuthenticationScheme), new[] { AuthService.UserRole });
                var signinTask = context.SignInAsync(cookie, authPropertis);

                var oauth = await oauthTask;
                if (oauth == null)
                {
                    var account = new Account(principal.Identity?.Name);
                    db.Accounts.Add(account);
                    await db.SaveChangesAsync();
                    oauth = new(oauthId, scheme) { AccountId = account.Id };
                    db.Add(oauth);
                    await db.SaveChangesAsync();
                }
                var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
                cache.Set(token, oauth.AccountId, cacheOptions);

                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogLoginScheme(oauthId, scheme);

                await signinTask;
                context.Response.Redirect("/");
                return;
            }

            await context.ChallengeAsync(scheme);
        });
    }
}

#region Configure
app
    .UseHttpsRedirection()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoint =>
    {
        endpoint.MapGrpcService<AnonymousService>();
        endpoint.MapGrpcService<AuthenticatedService>().RequireAuthorization("");
        MapAuthenticaters(endpoint, new[] { GitHubAuthenticationDefaults.AuthenticationScheme, MicrosoftAccountDefaults.AuthenticationScheme });
        endpoint.MapGet("/logout", async context =>
        {
            var res = await context.AuthenticateAsync();
            if (res.Succeeded && res.Principal?.Identity?.Name is string token)
            {
                var services = context.RequestServices;
                if (services.GetRequiredService<IMemoryCache>().TryGetValue(token, out string id))
                {
                    services.GetRequiredService<ILogger<Program>>().LogLogout(id);
                }
            }

            await context.SignOutAsync();
            context.Response.Redirect("/");
        });
    });
#endregion

app.Run();
