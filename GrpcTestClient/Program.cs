using Grpc.Net.Client;

using Whu.Lambda.Web;

using static System.Console;

try
{
    using var channel = GrpcChannel.ForAddress("https://localhost:7262");
    var client = new Anonymous.AnonymousClient(channel);
    WriteLine(client.SignupAsync(new()
    {
        Username = "test",
        Password = "test"
    }).GetAwaiter().GetResult().Value);
    WriteLine((await client.LoginAsync(new()
    {
        Username = "test",
        Password = "test"
    })).Value);
    var authed = new Authenticated.AuthenticatedClient(channel);
    WriteLine((await authed.PostArticleAsync(new()
    {
        Name = "114514",
    })).Value);
    var articles = client.GetArticles(new()).ResponseStream;
    while (await articles.MoveNext(default))
    {
        WriteLine(articles.Current.CreatedAt.ToDateTimeOffset());
    }
}
catch (Exception ex)
{
    WriteLine($"Exception {ex.Message}");
}