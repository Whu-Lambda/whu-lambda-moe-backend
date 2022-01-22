using Google.Protobuf.WellKnownTypes;

using Whu.Lambda.Web;

namespace GrpcServer.Mixin;

public static class ActivityExtension
{
    public static DAO.Activity ToDAO(this Activity a) =>
        new(a.Name, a.Content, a.Summary, a.Author, a.CoverUrl, a.Tags, a.Time, a.Place)
        {
            CreatedAt = a.CreatedAt.ToDateTime()
        };

    public static Activity ToDTO(this DAO.Activity a) =>
        new()
        {
            Name = a.Name,
            Summary = a.Summary,
            Content = a.Content,
            Author = a.Author,
            CreatedAt = Timestamp.FromDateTime(a.CreatedAt),
            CoverUrl = a.Cover,
            Tags = a.Tags,
            IsValid = true,
            Place = a.Place,
            Status = a.Status,
            Time = a.TimeSlot
        };
}
