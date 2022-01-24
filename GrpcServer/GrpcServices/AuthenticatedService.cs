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

    public AuthenticatedService(IMemoryCache cache, DbService db)
    {
        this.cache = cache;
        this.db = db;
    }

    public override async Task<Empty> Logout(Empty request, ServerCallContext context)
    {
        var httpContext = context.GetHttpContext();
        // May this be null?
        string token = httpContext.User.FindFirst(AuthService.KEY)!.Value;
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
