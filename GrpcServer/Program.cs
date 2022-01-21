using GrpcServer.GrpcServices;
using GrpcServer.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;

#region ConfigureServices
services.AddGrpc();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });
services
    .AddAuthorization(options =>
    {
    })
    .AddRouting()
    .AddDbContextPool<DbService>(option =>
    {
        option.UseSqlite(configuration.GetConnectionString("sqlite"));
    },
    100);
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
    });
#endregion

app.Run();
