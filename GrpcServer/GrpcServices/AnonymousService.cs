using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Whu.Lambda.Dto;

namespace GrpcServer.GrpcServices;

public class AnonymousService : Anonymous.AnonymousBase
{
    public override Task<Article> GetArticle(Int32Value request, ServerCallContext context)
    {
        return base.GetArticle(request, context);
    }

    public override Task<Activity> GetActivity(Int32Value request, ServerCallContext context)
    {
        return base.GetActivity(request, context);
    }
}
