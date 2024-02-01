namespace Core.Entities;

public sealed class Url
{
    public required long Id { get; set; }
    public required string OriginalUrl { get; set; }
    public required string ShortenedUrl { get; set; }
    public required DateTime CreatedAt { get; set; }
}
