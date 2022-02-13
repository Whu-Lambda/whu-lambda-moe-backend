using Grpc.Net.Client;

using System;

using Whu.Lambda.Moe.Dto;

using Xunit;

namespace Whu.Lambda.Moe.Backend.Test;

public class IntegrationTest
{
    const string Url = "http://localhost:5000";
    private readonly Anonymous.AnonymousClient anony;
    private readonly Authenticated.AuthenticatedClient authed;

    public IntegrationTest()
    {
        var channel = GrpcChannel.ForAddress(Url);
        anony = new Anonymous.AnonymousClient(channel);
        authed = new Authenticated.AuthenticatedClient(channel);
        anony.HealthCheck(new());
    }

    private static string RandString()
    {
        byte[] bytes = new byte[100];
        Random.Shared.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    [Fact]
    public void AnonyCheck()
    {
        var stream = anony.GetActivities(new());
        while (stream.ResponseStream.MoveNext(default).Result) ;
    }

    [Fact]
    public void AuthCheck() => authed.HealthCheck(new());

    [Fact]
    public void SampleTest()
    {
        var activity = new Activity() { Content = RandString() };
        int id = authed.PostActivity(activity).Value;
        var response = anony.GetActivity(new() { Value = id });
        Assert.True(response.Content == activity.Content);
    }
}
