using Whu.Lambda.Web;

namespace GrpcServer.Mixin;

public static class AccountExtension
{
    public static DAO.Account ToDAO(this Account a) =>
        new(a.Email, a.Password, a.Phone, a.Username, a.Avatar, bio: a.Bio);

    public static Account ToDTO(this DAO.Account a) =>
        new()
        {
            Avatar = a.Avatar,
            Bio = a.Bio,
            Email = a.Email,
            Password = a.Password,
            Phone = a.Phone,
            Username = a.Username,
            IsValid = true
        };

}
