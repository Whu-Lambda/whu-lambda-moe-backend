using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace GrpcServer.Services;

public class AuthService : AuthorizationHandler<AuthService.AuthRequirement>
{
    public const string KEY = "auth-token";
    public static readonly TimeSpan EXPIRATION = TimeSpan.FromDays(14);

    private readonly IMemoryCache cache;

    public class AuthRequirement : IAuthorizationRequirement { }

    public AuthService(IMemoryCache cache)
    {
        this.cache = cache;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        string? token = context.User.FindFirst(KEY)?.Value;
        if (cache.TryGetValue(token, out var _))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }
}
