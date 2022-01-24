using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using GrpcServer.Mixin;
using GrpcServer.Services;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;

using System.Security.Claims;

using Whu.Lambda.Web;

namespace GrpcServer.GrpcServices;

public class AnonymousService : Anonymous.AnonymousBase
{
    private readonly DbService dbService;
    private readonly IMemoryCache cache;

    public AnonymousService(DbService dbService, IMemoryCache cache)
    {
        this.dbService = dbService;
        this.cache = cache;
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

    public override async Task<BoolValue> Signup(Account request, ServerCallContext context)
    {
        dbService.Add(request.ToDAO());
        try
        {
            await dbService.SaveChangesAsync();
            return new() { Value = true };
        }
        catch (Exception)
        {
            return new();
        }
    }

    public override async Task<BoolValue> Login(LoginInfo request, ServerCallContext context)
    {
        var acc = await dbService.Accounts.Select(a => new { a.Username, a.Password }).FirstOrDefaultAsync(a => a.Username == request.Username);
        if (request.Password == acc?.Password)
        {
            string token = Guid.NewGuid().ToString();
            var expire = new MemoryCacheEntryOptions { SlidingExpiration = AuthService.EXPIRATION };
            // May GUID collide with you.
            // Previous token not dismissed.
            cache.Set(token, acc.Username, expire);
            await context.GetHttpContext().SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(AuthService.KEY, token) })));
            return new() { Value = true };
        }
        return new();
    }
}
