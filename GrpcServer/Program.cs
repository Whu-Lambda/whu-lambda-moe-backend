using GrpcServer.Services;

using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

#region ConfigureServices
services.AddGrpc();
services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option => 
    { 
    });
services
    .AddAuthorization(options => 
    { 
    })
    .AddRouting();
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
        endpoint.MapGrpcService<ActivityServiceImpl>();
        endpoint.MapGrpcService<ArticleServiceImpl>();
    });
#endregion

app.Run();
