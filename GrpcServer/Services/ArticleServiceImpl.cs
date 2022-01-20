using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Whu.Lambda.Dto;

namespace GrpcServer.Services;

public class ArticleServiceImpl: ArticleService.ArticleServiceBase
{
    public override Task<Article> GetArticle(Int32Value request, ServerCallContext context)
    {
        return base.GetArticle(request, context);
    }
}
