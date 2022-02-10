namespace Whu.Lambda.Moe.Backend.Dao;

public class OAuth
{
    public string OAuthId { get; set; }
    public int AccountId { get; set; }
    public string Scheme { get; set; }
    public OAuth(string oAuthId, string scheme)
    {
        OAuthId = oAuthId;
        Scheme = scheme;
    }
}
