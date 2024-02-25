using Core.Entities;
using Core.Mediatr.Commands;
using Infrastructure.Data;
using Infrastructure.Mediatr.Handlers.Urls;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests.Medaitr.Handlers.Urls;

public sealed class DeleteOldUrlsHandlerTests
{
    [Fact]
    public async void DeleteOldUrls_WhenCalled_ReturnDeletedUrls()
    {
        // Arrange
        var command = new DeleteOldUrlsCommand();

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
            .UseInMemoryDatabase(databaseName: "DeleteOldUrls")
            .Options;

        using (var context = new ApiDataContext(options))
        {
            context.Urls.AddRange(urls);
            context.SaveChanges();
        }

        using (var context = new ApiDataContext(options))
        {
            var handler = new DeleteOldUrlsHandler(context);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().HaveCount(2);
        }
    }
}
