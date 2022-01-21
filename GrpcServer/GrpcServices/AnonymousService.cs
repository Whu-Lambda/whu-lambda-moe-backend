using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using GrpcServer.Services;

using Whu.Lambda.Dto;

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
        return artile == null ?
            new Article() :
            new Article
            {
                Author = artile.Author,
                CreatedAt = Timestamp.FromDateTime(artile.CreatedAt),
                Content = artile.Content,
                CoverUrl = artile.Cover,
                IsValid = true,
                Name = artile.Name
            };
    }

    public override async Task<Activity> GetActivity(Int32Value request, ServerCallContext context)
    {
        var activity = await dbService.FindAsync<DAO.Activity>(request.Value);
        return activity == null ?
            new() :
            new Activity
            {
                IsValid = true,
                Name = activity.Name,
                Content = activity.Content,
                Place = activity.Place,
                Status = activity.Status,
                Time = activity.TimeSlot
            };
    }
}
