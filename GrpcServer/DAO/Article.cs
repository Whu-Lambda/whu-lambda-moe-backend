namespace GrpcServer.DAO;

public class Article
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string About { get; set; }
    public string Content { get; set; }
    public string Author { get; set; }
    public string Cover { get; set; }
    public string Tags { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public Article(string name, string about, string content, string author, string cover, string tags)
    {
        Name = name;
        About = about;
        Content = content;
        Author = author;
        Cover = cover;
        Tags = tags;
    }
}
