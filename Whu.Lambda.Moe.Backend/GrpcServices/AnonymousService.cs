using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Whu.Lambda.Moe.Backend.Mixin;
using Whu.Lambda.Moe.Backend.Services;
using Whu.Lambda.Moe.Dto;

namespace Whu.Lambda.Moe.Backend.GrpcServices;

public class AnonymousService : Anonymous.AnonymousBase
{
    private readonly DbService dbService;

    public AnonymousService(DbService dbService) => this.dbService = dbService;
    public async override Task<Article> GetArticle(Int32Value request, ServerCallContext context)
    {
        var artile = await dbService.FindAsync<Dao.Article>(request.Value);
        return artile?.ToDTO() ?? new();
    }

    public async override Task<Activity> GetActivity(Int32Value request, ServerCallContext context)
    {
        var activity = await dbService.FindAsync<Dao.Activity>(request.Value);
        return activity?.ToDTO() ?? new();
    }

    public async override Task GetActivities(Empty request, IServerStreamWriter<Activity> responseStream, ServerCallContext context)
    {
        await foreach (var activity in dbService.Activities)
        {
            await responseStream.WriteAsync(activity.ToDTO());
        }
    }

    public async override Task GetArticles(Empty request, IServerStreamWriter<Article> responseStream, ServerCallContext context)
    {
        await foreach (var article in dbService.Articles)
        {
            await responseStream.WriteAsync(article.ToDTO());
        }
    }
}
