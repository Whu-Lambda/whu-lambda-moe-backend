using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Whu.Lambda.Moe.Backend.Services;

public class AuthService : AuthorizationHandler<AuthService.AuthRequirement>
{
    public const string WhuLambdaScheme = "whu-lambda-auth";

    private readonly IMemoryCache cache;
    //private readonly ILogger<AuthService> logger;

    public class AuthRequirement : IAuthorizationRequirement { }

    public AuthService(IMemoryCache cache/*, ILogger<AuthService> logger*/) => this.cache = cache;//this.logger = logger;

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        string? token = context.User.FindFirst(WhuLambdaScheme)?.Value;
        if (token is not null && cache.TryGetValue(token, out _))
        {
            //logger.LogAuthPassed(username, token);
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
