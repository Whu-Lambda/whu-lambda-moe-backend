global using Microsoft.EntityFrameworkCore;

using AspNet.Security.OAuth.GitHub;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

using System.Security.Claims;

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
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = configuration["Microsoft:ClientID"];
        options.ClientSecret = configuration["Microsoft:ClientSecret"];
        options.CallbackPath = "/callback/auth/Microsoft";
    });
serviceBuilder
    .AddAuthorization(options =>
    {
        var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddRequirements(new AuthService.AuthRequirement())
            .Build();
        options.AddPolicy(AuthService.WhuLambdaScheme, policy);
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
    foreach (string scheme in schemes)
    {
        endpoint.MapGet($"/login/{scheme}", async context =>
        {
            var res = await context.AuthenticateAsync();
            if (res.Principal?.FindFirst(AuthService.WhuLambdaScheme)?.Value is string token)
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
            string? name = principal?.Identity?.Name;
            if (res.Succeeded && principal != null && name != null)
            {
                var db = context.RequestServices.GetRequiredService<DbService>();
                var oauthTask = db.OAuths.FirstOrDefaultAsync(oauth => oauth.Name == name && oauth.Scheme == scheme);

                token = Guid.NewGuid().ToString();
                principal.AddIdentity(new ClaimsIdentity(new Claim[] { new(AuthService.WhuLambdaScheme, token) }, CookieAuthenticationDefaults.AuthenticationScheme));
                var signinTask = context.SignInAsync(principal, new AuthenticationProperties()
                {
                    ExpiresUtc = DateTimeOffset.Now.AddDays(14),
                    AllowRefresh = true,
                    IsPersistent = true
                });

                var oauth = await oauthTask;
                var cache = context.RequestServices.GetRequiredService<IMemoryCache>();
                if (oauth == null)
                {
                    var account = new Account(name);
                    db.Accounts.Add(account);
                    await db.SaveChangesAsync();
                    oauth = new(name, scheme) { AccountId = account.Id };
                    db.Add(oauth);
                    await db.SaveChangesAsync();
                }
                cache.Set(token, oauth.AccountId, new MemoryCacheEntryOptions() { SlidingExpiration = TimeSpan.FromDays(14) });
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
        endpoint.MapGrpcService<AuthenticatedService>().RequireAuthorization();
        MapAuthenticaters(endpoint, new[] { GitHubAuthenticationDefaults.AuthenticationScheme, MicrosoftAccountDefaults.AuthenticationScheme });
        //endpoint.MapGet("/login", context =>
        //{
        //    context.Response.Redirect("/login/Microsoft");
        //    return Task.CompletedTask;
        //});
        endpoint.MapGet("/logout", async context =>
        {
            await context.SignOutAsync();
            context.Response.Redirect("/");
        });

    });
#endregion

app.Run();
