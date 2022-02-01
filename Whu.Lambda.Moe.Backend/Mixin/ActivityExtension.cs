using Google.Protobuf.WellKnownTypes;

using Whu.Lambda.Moe.Dto;

namespace Whu.Lambda.Moe.Backend.Mixin;

public static class ActivityExtension
{
    public static Dao.Activity ToDAO(this Activity a) =>
        new(a.Name, a.Content, a.Summary, a.Author, a.CoverUrl, a.Tags, a.Time, a.Place, a.Status);

    public static Activity ToDTO(this Dao.Activity a) =>
        new()
        {
            Name = a.Name,
            Summary = a.Summary,
            Content = a.Content,
            Author = a.Author,
            CreatedAt = Timestamp.FromDateTimeOffset(a.CreatedAt),
            CoverUrl = a.Cover,
            Tags = a.Tags,
            IsValid = true,
            Place = a.Place,
            Status = a.Status,
            Time = a.TimeSlot,
            Id = a.Id
        };
}
