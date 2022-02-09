namespace Whu.Lambda.Moe.Backend.Dao;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Avatar { get; set; }
    public string? Bio { get; set; }
    public int Reputation { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; }
    public bool IsStaff { get; set; }
    public bool IsSuperuser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastLogin { get; set; }

    public Account(string? username = null, string avatar = "avatar/default.jpg")
    {
        Username = username ?? Guid.NewGuid().ToString();
        Avatar = avatar;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
