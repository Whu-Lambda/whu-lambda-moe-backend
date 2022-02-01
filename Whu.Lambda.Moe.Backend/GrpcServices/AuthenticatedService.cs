using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

using Whu.Lambda.Moe.Backend.Mixin;
using Whu.Lambda.Moe.Backend.Services;
using Whu.Lambda.Moe.Dto;

namespace Whu.Lambda.Moe.Backend.GrpcServices;

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

    public async override Task<Empty> Logout(Empty request, ServerCallContext context)
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

    public async override Task<BoolValue> PostActivity(Activity request, ServerCallContext context)
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

    public async override Task<BoolValue> PostArticle(Article request, ServerCallContext context)
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
