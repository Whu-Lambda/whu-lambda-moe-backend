﻿namespace GrpcServer.DAO;

public class Activity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Content { get; set; }
    public string Summary { get; set; }
    public string Author { get; set; }
    public string Cover { get; set; }
    public string Tags { get; set; }
    public string Status { get; set; }
    public string TimeSlot { get; set; }
    public string Place { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }

    public Activity(string name, string content, string summary, string author, string cover, string tags, string timeSlot, string place, string status = "open", DateTimeOffset createdAt = default, DateTimeOffset updatedAt = default)
    {
        Name = name;
        Content = content;
        Summary = summary;
        Author = author;
        Cover = cover;
        Tags = tags;
        TimeSlot = timeSlot;
        Place = place;
        Status = status;
        CreatedAt = createdAt == default ? DateTime.Now : createdAt;
        UpdatedAt = updatedAt == default ? DateTime.Now : updatedAt;
    }
}
