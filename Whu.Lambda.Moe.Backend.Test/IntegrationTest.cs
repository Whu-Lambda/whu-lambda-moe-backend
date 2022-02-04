using Grpc.Core;
using Grpc.Net.Client;

using System;
using System.Threading.Tasks;

using Whu.Lambda.Moe.Dto;

using Xunit;

namespace Whu.Lambda.Moe.Backend.Test;

public class IntegrationTest
{
    private static string RandString()
    {
        byte[] bytes = new byte[100];
        Random.Shared.NextBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    [Fact]
    public async Task SampleTest()
    {
        var channel = GrpcChannel.ForAddress("https://localhost:7262");
        var anony = new Anonymous.AnonymousClient(channel);
        var account = new Account() { Username = RandString(), Password = RandString() };
        anony.Signup(account);
        anony.Login(new() { Username = account.Username, Password = account.Password });
        var authed = new Authenticated.AuthenticatedClient(channel);
        var activity = new Activity() { Content = RandString() };
        authed.PostActivity(activity);
        var response = anony.GetActivities(new()).ResponseStream;
        bool passed = false;
        while(await response.MoveNext())
        {
            if(response.Current.Content == activity.Content)
            {
                passed = true;
                break;
            }
        }
        Assert.True(passed);
    }
}
