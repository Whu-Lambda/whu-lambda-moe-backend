global using Microsoft.EntityFrameworkCore;

using AspNet.Security.OAuth.GitHub;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;

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
        options.DefaultScheme = MicrosoftAccountDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = options.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
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
        options.CallbackPath = "/callback/auth/Github";
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = configuration["Microsoft:ClientID"];
        options.ClientSecret = configuration["Microsoft:ClientSecret"];
        options.CallbackPath = "/callback/auth/Microsoft";
    });
serviceBuilder
    .AddAuthorization()
    .AddRouting()
    //.AddMemoryCache()
    .AddDbContextPool<DbService>(option => option.UseSqlite(configuration.GetConnectionString("sqlite")));
#endregion

var app = builder.Build();

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
        endpoint.MapGet("/login/Github", async context =>
        {
            var res = await context.AuthenticateAsync(GitHubAuthenticationDefaults.AuthenticationScheme);

            if (res.Succeeded)
            {
                context.Response.Redirect("/");
            }
            else
            {
                await context.ChallengeAsync(GitHubAuthenticationDefaults.AuthenticationScheme);
            }
        });
        endpoint.MapGet("/login/Microsoft", async context =>
        {
            var res = await context.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
            if (res.Succeeded)
            {
                context.Response.Redirect("/");
            }
            else
            {
                await context.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme);
            }
        });
        endpoint.MapGet("/callback/auth/Github", context =>
        {
            context.Response.Redirect("/");
            return Task.CompletedTask;
        });
        endpoint.MapGet("/callback/auth/Microsoft", context =>
        {
            context.Response.Redirect("/");
            return Task.CompletedTask;
        });
        endpoint.MapGet("/login", context =>
        {
            context.Response.Redirect("/login/Microsoft");
            return Task.CompletedTask;
        });
        endpoint.MapGet("/logout", async context =>
        {
            await context.SignOutAsync();
            context.Response.Redirect("/");
        });

    });
#endregion

app.Run();
