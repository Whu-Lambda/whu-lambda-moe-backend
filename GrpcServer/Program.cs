global using Microsoft.EntityFrameworkCore;

using GrpcServer.GrpcServices;
using GrpcServer.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;

#region ConfigureServices
services.AddGrpc();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        option.ExpireTimeSpan = AuthService.EXPIRATION;
        option.SlidingExpiration = true;
    });
services
    .AddAuthorization(options =>
    {
        string noAnony = "NoAnony";
        options.AddPolicy(noAnony, policy =>
        {
            policy.AddRequirements(new AuthService.AuthRequirement());
        });
        // It won't be null, right?
        options.DefaultPolicy = options.GetPolicy(noAnony)!;
    })
    .AddRouting()
    .AddDbContextPool<DbService>(option =>
    {
        option.UseSqlite(configuration.GetConnectionString("sqlite"));
    })
    .AddSingleton<IAuthorizationHandler, AuthService>();
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
        endpoint.MapGrpcService<AuthenticatedService>();
    });
#endregion

app.Run();
