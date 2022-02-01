global using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

using Whu.Lambda.Moe.Backend.GrpcServices;
using Whu.Lambda.Moe.Backend.Services;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;

#region ConfigureServices
services.AddGrpc();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        option.ExpireTimeSpan = AuthService.Expiration;
        option.SlidingExpiration = true;
    });
services
    .AddAuthorization(options =>
    {
        var policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new AuthService.AuthRequirement())
            .Build();
        options.AddPolicy(AuthService.PolicyName, policy);
        options.DefaultPolicy = policy;
    })
    .AddRouting()
    .AddDbContextPool<DbService>(option => option.UseSqlite(configuration.GetConnectionString("sqlite")))
    .AddSingleton<IAuthorizationHandler, AuthService>()
    .AddMemoryCache();
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
    });
#endregion

app.Run();
