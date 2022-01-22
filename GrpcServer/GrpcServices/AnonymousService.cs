using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using GrpcServer.Mixin;
using GrpcServer.Services;

using Whu.Lambda.Web;

namespace GrpcServer.GrpcServices;

public class AnonymousService : Anonymous.AnonymousBase
{
    private readonly DbService dbService;

    public AnonymousService(DbService dbService)
    {
        this.dbService = dbService;
    }
    public override async Task<Article> GetArticle(Int32Value request, ServerCallContext context)
    {
        var artile = await dbService.FindAsync<DAO.Article>(request.Value);
        return artile?.ToDTO() ?? new();
    }

    public override async Task<Activity> GetActivity(Int32Value request, ServerCallContext context)
    {
        var activity = await dbService.FindAsync<DAO.Activity>(request.Value);
        return activity?.ToDTO() ?? new();
    }
}
