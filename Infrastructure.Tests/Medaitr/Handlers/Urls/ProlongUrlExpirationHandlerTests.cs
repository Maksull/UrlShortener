using Core.Entities;
using Core.Mediatr.Commands;
using Infrastructure.Data;
using Infrastructure.Mediatr.Handlers.Urls;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Medaitr.Handlers.Urls;

public sealed class ProlongUrlExpirationHandlerTests
{
    [Fact]
    public async void ProlongUrlExpiration_WhenCalled_ReturnUrl()
    {
        // Arrange
        var command = new ProlongUrlExpirationCommand(1, TimeSpan.FromMinutes(1));

        Url[] urls = [
            new() {
                    Id = 1,
                    OriginalUrl = "https://example1.com",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    ExpireAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new() {
                Id = 2,
                OriginalUrl = "https://example2.com",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                ExpireAt = DateTime.UtcNow.AddMinutes(-5)
            },
            new() {
                Id = 3,
                OriginalUrl = "https://example2.com",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5),
                ExpireAt = DateTime.UtcNow.AddMinutes(5)
            },
        ];

        var options = new DbContextOptionsBuilder<ApiDataContext>()
            .UseInMemoryDatabase(databaseName: "ProlongUrlExpiration1")
            .Options;

        using (var context = new ApiDataContext(options))
        {
            context.Urls.AddRange(urls);
            context.SaveChanges();
        }

        using (var context = new ApiDataContext(options))
        {
            var handler = new ProlongUrlExpirationHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(urls[0], opts =>
                opts.Excluding(u => u.ExpireAt));
            result?.ExpireAt.Should().BeCloseTo(urls[0].ExpireAt.AddMinutes(5), TimeSpan.FromMinutes(2));
        }
    }

    [Fact]
    public async void ProlongUrlExpiration_WhenCalled_ReturnNull()
    {
        // Arrange
        var command = new ProlongUrlExpirationCommand(2, TimeSpan.FromMinutes(1));

        Url[] urls = [
            new() {
                    Id = 1,
                    OriginalUrl = "https://example1.com",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                    ExpireAt = DateTime.UtcNow.AddMinutes(-5)
            }
        ];

        var options = new DbContextOptionsBuilder<ApiDataContext>()
            .UseInMemoryDatabase(databaseName: "ProlongUrlExpiration2")
            .Options;

        using (var context = new ApiDataContext(options))
        {
            context.Urls.AddRange(urls);
            context.SaveChanges();
        }

        using (var context = new ApiDataContext(options))
        {
            var handler = new ProlongUrlExpirationHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }
    }
}
