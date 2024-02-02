using Core.Entities;
using HashidsNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

internal sealed class UrlConfiguration : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        var hashIds = new Hashids("UrlShortener");

        Url[] urls =
        {
            new()
            {
                Id = 1,
                OriginalUrl = "https://www.youtube.com/",
                ShortenedUrl = hashIds.EncodeLong(1),
                CreatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = 2,
                OriginalUrl = "https://learn.microsoft.com/",
                ShortenedUrl = hashIds.EncodeLong(2),
                CreatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = 3,
                OriginalUrl = "https://www.linkedin.com/feed/",
                ShortenedUrl = hashIds.EncodeLong(3),
                CreatedAt = DateTime.UtcNow,
            },
        };

        builder.HasData(urls);
    }
}
