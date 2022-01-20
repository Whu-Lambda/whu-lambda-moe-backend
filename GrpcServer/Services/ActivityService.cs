using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Whu.Lambda.Dto;

namespace GrpcServer.Services;

public class ActivityServiceImpl : ActivityService.ActivityServiceBase
{
    public override Task<Activity> GetActivity(Int32Value request, ServerCallContext context)
    {
        return base.GetActivity(request, context);
    }
}