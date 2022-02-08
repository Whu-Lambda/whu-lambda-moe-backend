using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Whu.Lambda.Moe.Backend.Mixin;
using Whu.Lambda.Moe.Backend.Services;
using Whu.Lambda.Moe.Dto;

namespace Whu.Lambda.Moe.Backend.GrpcServices;

public class AuthenticatedService : Authenticated.AuthenticatedBase
{
    private readonly DbService db;

    public AuthenticatedService(DbService db) =>
        //this.cache = cache;
        this.db = db;

    public async override Task<Int32Value> PostActivity(Activity request, ServerCallContext context)
    {
        var entry = db.Add(request.ToDAO());
        try
        {
            _ = await db.SaveChangesAsync();
            return new() { Value = entry.Entity.Id };
        }
        catch (Exception)
        {
            return new() { Value = -1 };
        }
    }

    public async override Task<Int32Value> PostArticle(Article request, ServerCallContext context)
    {
        var entry = db.Add(request.ToDAO());
        try
        {
            _ = await db.SaveChangesAsync();
            return new() { Value = entry.Entity.Id };
        }
        catch (Exception)
        {
            return new() { Value = -1 };
        }
    }
}
