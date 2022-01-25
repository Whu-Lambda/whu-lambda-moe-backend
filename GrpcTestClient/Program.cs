using Whu.Lambda.Web;
using static System.Console;

using Grpc.Net.Client;
using Google.Protobuf.WellKnownTypes;

try
{
    using var channel = GrpcChannel.ForAddress("https://localhost:7262");
    var client = new Anonymous.AnonymousClient(channel);
    WriteLine((await client.SignupAsync(new()
    {
        Username = "test",
        Password = "test",
        IsValid = true
    })).Value);
    WriteLine((await client.LoginAsync(new()
    {
        Username = "test",
        Password = "test"
    })).Value);
    var authed = new Authenticated.AuthenticatedClient(channel);
    WriteLine((await authed.PostActivityAsync(new()
    {
        IsValid = true,
        Name = "114514",
    })).Value);
    WriteLine((await client.GetActivityAsync(new()
    {
        Value = 1
    })).Name);
}
catch (Exception ex)
{
    WriteLine($"Exception {ex.Message}");
}