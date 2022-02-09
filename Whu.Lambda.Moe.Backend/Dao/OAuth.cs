namespace Whu.Lambda.Moe.Backend.Dao;

public class OAuth
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int AccountId { get; set; }
    public string Scheme { get; set; }
    public OAuth(string name, string scheme)
    {
        Name = name;
        Scheme = scheme;
    }
}
