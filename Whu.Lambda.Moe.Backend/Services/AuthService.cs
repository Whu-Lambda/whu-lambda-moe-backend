using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace Whu.Lambda.Moe.Backend.Services;

public class AuthService : AuthorizationHandler<AuthService.AuthRequirement>
{
    public const string UserRole = "User";

    private readonly IMemoryCache cache;
    private readonly bool isDev;
    private readonly ILogger<AuthService> logger;

    public class AuthRequirement : IAuthorizationRequirement { }

    public AuthService(IMemoryCache cache, IWebHostEnvironment environment, ILogger<AuthService> logger)
    {
        this.cache = cache;
        isDev = environment.IsDevelopment();
        this.logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        if (isDev)
        {
            context.Succeed(requirement);
        }
        if (context.User.Identity?.Name is string token && cache.TryGetValue(token, out string id))
        {
            context.Succeed(requirement);
            logger.LogLogin(id);
        }
        return Task.CompletedTask;
    }
}
