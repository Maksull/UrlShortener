using Core.Entities;
using Infrastructure.Data.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApiDataContext : DbContext
{
    public ApiDataContext() { }
    public ApiDataContext(DbContextOptions<ApiDataContext> options) : base(options) { }

    public DbSet<Url> Urls => Set<Url>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new UrlConfiguration());

        builder.Entity<Url>(build =>
        {
            build.HasKey(x => x.Id);
            build.HasIndex(u => u.OriginalUrl).IsUnique();
            build.HasIndex(u => u.ShortenedUrl).IsUnique();
        });
    }
}
