using Grpc.Net.Client;

using System;

using Whu.Lambda.Moe.Dto;

using Xunit;

namespace Whu.Lambda.Moe.Backend.Test;

public class IntegrationTest
{
    const string Url = "https://localhost:14514";
    private readonly Anonymous.AnonymousClient anony;
    private readonly Authenticated.AuthenticatedClient authed;

    public IntegrationTest()
    {
        var channel = GrpcChannel.ForAddress(Url);
        anony = new Anonymous.AnonymousClient(channel);
        authed = new Authenticated.AuthenticatedClient(channel);
    }

    private static string RandString()
    {
        byte[] bytes = new byte[100];
        Random.Shared.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    [Fact]
    public void HealthCheck() => anony.HealthCheck(new());

    [Fact]
    public void AnonyCheck() => anony.GetActivities(new());

    [Fact]
    public void AuthCheck() => authed.PostActivity(new());

    [Fact]
    public void SampleTest()
    {
        var activity = new Activity() { Content = RandString() };
        int id = authed.PostActivity(activity).Value;
        var response = anony.GetActivity(new() { Value = id });
        Assert.True(response.Content == activity.Content);
    }
}
