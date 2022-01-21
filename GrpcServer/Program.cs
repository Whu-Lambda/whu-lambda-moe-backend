using GrpcServer.GrpcServices;
using GrpcServer.Services;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Text;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration configuration = builder.Configuration;

var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("SymmetricKey")));

#region ConfigureServices
services.AddGrpc();
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new()
        {
            IssuerSigningKey = SecurityKey,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true
        };
    });
services
    .AddAuthorization(options =>
    {
    })
    .AddRouting()
    .AddDbContextPool<DbService>(option =>
    {
        option.UseSqlite(configuration.GetConnectionString("sqlite"));
    });
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
