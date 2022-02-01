using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Whu.Lambda.Moe.Backend.Services;

public class AuthService : AuthorizationHandler<AuthService.AuthRequirement>
{
    public const string Key = "auth-token", PolicyName = "NoAnony";
    public static readonly TimeSpan Expiration = TimeSpan.FromDays(14);

    private readonly IMemoryCache cache;
    private readonly ILogger<AuthService> logger;

    public class AuthRequirement : IAuthorizationRequirement { }

    public AuthService(IMemoryCache cache, ILogger<AuthService> logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        string? token = context.User.FindFirst(Key)?.Value;
        if (cache.TryGetValue(token, out string username))
        {
            logger.LogInformation("{username} passed with token {token}.", username, token);
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }
}
