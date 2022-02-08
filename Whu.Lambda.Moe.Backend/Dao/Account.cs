//namespace Whu.Lambda.Moe.Backend.Dao;

//public class Account
//{
//    public int Id { get; set; }
//    public string Username { get; set; }
//    public string Avatar { get; set; }
//    public string Bio { get; set; }
//    public int Reputation { get; set; }
//    public string Email { get; set; }
//    public string Password { get; set; }
//    public string Phone { get; set; }
//    public bool IsActive { get; set; }
//    public bool IsStaff { get; set; }
//    public bool IsSuperuser { get; set; }
//    public DateTime CreatedAt { get; set; }
//    public DateTime UpdatedAt { get; set; }
//    public DateTime LastLogin { get; set; }

//    public Account(string email, string password, string phone, string? username = null, string avatar = "avatar/default.jpg", DateTime createdAt = default, DateTime updatedAt = default, string bio = "")
//    {
//        Username = username ?? Guid.NewGuid().ToString();
//        Email = email;
//        Password = password;
//        Phone = phone;
//        Avatar = avatar;
//        CreatedAt = createdAt == default ? DateTime.UtcNow : createdAt;
//        UpdatedAt = updatedAt == default ? DateTime.UtcNow : updatedAt;
//        Bio = bio;
//    }
//}
