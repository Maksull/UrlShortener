using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configuration;

internal sealed class UrlConfiguration : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        Url[] urls =
        {
            new()
            {
                Id = 1,
                OriginalUrl = "https://www.youtube.com/",
                ShortenedUrl = "",
                CreatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = 2,
                OriginalUrl = "https://www.youtube.com/",
                ShortenedUrl = "",
                CreatedAt = DateTime.UtcNow,
            },
            new()
            {
                Id = 3,
                OriginalUrl = "https://www.youtube.com/",
                ShortenedUrl = "",
                CreatedAt = DateTime.UtcNow,
            },
        };

        builder.HasData(urls);
    }
}
