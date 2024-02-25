using Core.Entities;
using Core.Mediatr.Commands;
using HashidsNet;
using Infrastructure.Data;
using Infrastructure.Mediatr.Handlers.Urls;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace Infrastructure.Tests.Medaitr.Handlers.Urls;

public sealed class ShortUrlHandlerTests
{
    private readonly IMediator _mediator;

    public ShortUrlHandlerTests()
    {
        _mediator = Substitute.For<IMediator>(); ;
    }

    [Fact]
    public async void ShortUrl_WhenNoUrlInDb_ReturnCode()
    {
        // Arrange
        Hashids hashids = new();
        ShortUrlCommand command = new("https://localhost:7167", "https://www.youtube.com/");

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
            .UseInMemoryDatabase(databaseName: "ShortUrl1")
            .Options;

        using (ApiDataContext context = new(options))
        {
            context.Urls.AddRange(urls);
            context.SaveChanges();
        }

        using (ApiDataContext context = new(options))
        {
            var handler = new ShortUrlHandler(context, hashids, _mediator);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }
    }

    [Fact]
    public async void ShortUrl_WhenUrlInDb_ProlongReturnCode()
    {
        // Arrange
        Hashids hashids = new();
        ShortUrlCommand command = new("https://localhost:7167", "https://www.youtube.com/");

        Url[] urls = [
            new() {
                Id = 1,
                OriginalUrl = "https://www.youtube.com/",
                Code = hashids.EncodeLong(1),
                ShortenedUrl = $"https://localhost:7167/{hashids.EncodeLong(1)}",
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
            .UseInMemoryDatabase(databaseName: "ShortUrl2")
            .Options;

        using (ApiDataContext context = new(options))
        {
            context.Urls.AddRange(urls);
            context.SaveChanges();
        }

        using (ApiDataContext context = new(options))
        {
            var handler = new ShortUrlHandler(context, hashids, _mediator);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().BeEquivalentTo(urls[0].ShortenedUrl);
        }
    }
}
