using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using GrpcServer.Mixin;
using GrpcServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

using Whu.Lambda.Web;

namespace GrpcServer.GrpcServices;

public class AuthenticatedService : Authenticated.AuthenticatedBase
{
    private readonly IMemoryCache cache;
    private readonly DbService db;
    private readonly ILogger<AuthenticatedService> logger;

    public AuthenticatedService(IMemoryCache cache, DbService db, ILogger<AuthenticatedService> logger)
    {
        this.cache = cache;
        this.db = db;
        this.logger = logger;
    }

    public override async Task<Empty> Logout(Empty request, ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        var claim = httpContext.User.FindFirst(AuthService.Key);
        if (claim == null)
        {
            logger.LogError("Token not found, but passed auth.");
            return new();
        }
        string token = claim.Value;
        cache.Remove(token);
        await httpContext.SignOutAsync();
        return new();
    }

    public override async Task<BoolValue> PostActivity(Activity request, ServerCallContext context)
    {
        db.Add(request.ToDAO());
        try
        {
            await db.SaveChangesAsync();
            return new() { Value = true };
        }
        catch (Exception)
        {
            return new();
        }
    }

    public override async Task<BoolValue> PostArticle(Article request, ServerCallContext context)
    {
        db.Add(request.ToDAO());
        try
        {
            await db.SaveChangesAsync();
            return new() { Value = true };
        }
        catch (Exception)
        {
            return new();
        }
    }
}
