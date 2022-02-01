using Whu.Lambda.Moe.Dto;

namespace Whu.Lambda.Moe.Backend.Mixin;

public static class AccountExtension
{
    public static Dao.Account ToDAO(this Account a) =>
        new(a.Email, a.Password, a.Phone, a.Username, a.Avatar, bio: a.Bio);

    public static Account ToDTO(this Dao.Account a) =>
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
