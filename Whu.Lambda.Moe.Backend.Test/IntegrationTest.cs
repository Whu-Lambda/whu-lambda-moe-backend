using Grpc.Net.Client;

using System;

using Whu.Lambda.Moe.Dto;

using Xunit;

namespace Whu.Lambda.Moe.Backend.Test;

public class IntegrationTest
{
    const string Url = "https://localhost:14514";

    private static string RandString()
    {
        byte[] bytes = new byte[100];
        Random.Shared.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    [Fact]
    public void SampleTest()
    {
        var channel = GrpcChannel.ForAddress(Url);
        var anony = new Anonymous.AnonymousClient(channel);
        var authed = new Authenticated.AuthenticatedClient(channel);
        var activity = new Activity() { Content = RandString() };
        int id = authed.PostActivity(activity).Value;
        var response = anony.GetActivity(new() { Value = id });
        Assert.True(response.Content == activity.Content);
    }
}
