
namespace Core.Entities;

public sealed class Url
{
    public long Id { get; set; }
    public required string OriginalUrl { get; set; }
    public string ShortenedUrl { get; set; } = string.Empty;
    public required DateTime CreatedAt { get; set; }
}
