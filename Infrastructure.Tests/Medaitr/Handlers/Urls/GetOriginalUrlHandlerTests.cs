using Core.Entities;
using Core.Mediatr.Queries;
using HashidsNet;
using Infrastructure.Data;
using Infrastructure.Mediatr.Handlers.Urls;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace Infrastructure.Tests.Medaitr.Handlers.Urls;

public sealed class GetOriginalUrlHandlerTests
{
    private readonly ApiDataContext _apiDataContext;
    private readonly IHashids _hashids;

    public GetOriginalUrlHandlerTests()
    {
        _apiDataContext = Substitute.For<ApiDataContext>();
        _hashids = new Hashids();
    }


    [Fact]
    public async void GetOriginalUrl_WhenCalled_ReturnOriginalUrl()
    {
        // Arrange
        long urlId = 1;
        var code = _hashids.EncodeLong(urlId);
        var expectedOriginalUrl = "https://example.com";

        var query = new GetOriginalUrlQuery(code);
        Url url = new()
        {
            Id = urlId,
            OriginalUrl = expectedOriginalUrl,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            ExpireAt = DateTime.UtcNow.AddMinutes(5)
        };

        _apiDataContext.Urls.FindAsync(urlId, Arg.Any<CancellationToken>()).Returns(url);

        var handler = new GetOriginalUrlHandler(_apiDataContext, _hashids);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().Be(expectedOriginalUrl);
    }

    [Fact]
    public async void GetOriginalUrl_WhenCalled_ReturnNull()
    {
        // Arrange
        long urlId = 1;
        var code = _hashids.EncodeLong(urlId);
        var query = new GetOriginalUrlQuery(code);

        _apiDataContext.Urls.FindAsync(Arg.Any<long>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var handler = new GetOriginalUrlHandler(_apiDataContext, _hashids);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }
}
