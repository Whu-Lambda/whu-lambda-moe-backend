using Google.Protobuf.WellKnownTypes;

using Whu.Lambda.Moe.Dto;

namespace Whu.Lambda.Moe.Backend.Mixin;

public static class ArticleExtension
{
    public static Dao.Article ToDAO(this Article a) =>
        new(a.Name, a.About, a.Content, a.Author, a.CoverUrl, a.Tags);

    public static Article ToDTO(this Dao.Article a) =>
        new()
        {
            Name = a.Name,
            About = a.About,
            Content = a.Content,
            Author = a.Author,
            CreatedAt = Timestamp.FromDateTimeOffset(a.CreatedAt),
            CoverUrl = a.Cover,
            Tags = a.Tags,
            IsValid = true,
            Id = a.Id
        };
}
