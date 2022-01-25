using Google.Protobuf.WellKnownTypes;

using Whu.Lambda.Web;

namespace GrpcServer.Mixin;

public static class ArticleExtension
{
    public static DAO.Article ToDAO(this Article a) =>
        new(a.Name, a.About, a.Content, a.Author, a.CoverUrl, a.Tags);

    public static Article ToDTO(this DAO.Article a) =>
        new()
        {
            Name = a.Name,
            About = a.About,
            Content = a.Content,
            Author = a.Author,
            CreatedAt = Timestamp.FromDateTimeOffset(a.CreatedAt),
            CoverUrl = a.Cover,
            Tags = a.Tags,
            IsValid = true
        };
}
