using Whu.Lambda.Dto;

using Grpc.Net.Client;


try
{
    using var channel = GrpcChannel.ForAddress("https://localhost:7262");
    var client = new Anonymous.AnonymousClient(channel);
    var reply = await client.GetActivityAsync(new());
    Console.WriteLine(reply.IsValid);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}