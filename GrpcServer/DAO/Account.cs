namespace GrpcServer.DAO;

public class Account
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Avatar { get; set; }
    public string? Bio { get; set; }
    public int Reputation { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Phone { get; set; }
    public bool IsActive { get; set; }
    public bool IsStaff { get; set; }
    public bool IsSuperuser { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime LastLogin { get; set; }

    public Account(string email, string password, string phone)
    {
        Email = email;
        Password = password;
        Phone = phone;
        Username = Guid.NewGuid().ToString();
        Avatar = "avatar/default.jpg";
    }
}
