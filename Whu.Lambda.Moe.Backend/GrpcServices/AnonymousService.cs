﻿using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;

using System.Security.Claims;

using Whu.Lambda.Moe.Backend.Mixin;
using Whu.Lambda.Moe.Backend.Services;
using Whu.Lambda.Moe.Dto;

namespace Whu.Lambda.Moe.Backend.GrpcServices;

public class AnonymousService : Anonymous.AnonymousBase
{
    private readonly DbService dbService;
    private readonly IMemoryCache cache;
    private readonly ILogger<AnonymousService> logger;

    public AnonymousService(DbService dbService, IMemoryCache cache, ILogger<AnonymousService> logger)
    {
        this.dbService = dbService;
        this.cache = cache;
        this.logger = logger;
    }
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

    public async override Task<BoolValue> Signup(Account request, ServerCallContext context)
    {
        dbService.Add(request.ToDAO());
        try
        {
            await dbService.SaveChangesAsync();
            logger.LogSignup(request.Username);
            return new() { Value = true };
        }
        catch (Exception)
        {
            return new();
        }
    }

    public async override Task<BoolValue> Login(LoginInfo request, ServerCallContext context)
    {
        var acc = await dbService.Accounts.Select(a => new { a.Username, a.Password }).FirstOrDefaultAsync(a => a.Username == request.Username);
        if (request.Password == acc?.Password)
        {
            string token = Guid.NewGuid().ToString();
            var expire = new MemoryCacheEntryOptions { SlidingExpiration = AuthService.Expiration };
            // May GUID collide with you.
            // Previous token not dismissed.
            cache.Set(token, acc.Username, expire);
            await context.GetHttpContext().SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new(AuthService.Key, token) }, CookieAuthenticationDefaults.AuthenticationScheme)));
            logger.LogLogin(acc.Username, token);
            return new() { Value = true };
        }
        return new();
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
