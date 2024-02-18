using Core.Contracts.UrlEndpoints;
using Core.Mediatr.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using UrlShortenerApi.Endpoints;

namespace UrlShortenerApi.Tests.Endpoints;

public sealed class UrlEndpointsTests
{
    private readonly IMediator _mediator;

    public UrlEndpointsTests()
    {
        _mediator = Substitute.For<IMediator>();
    }


    [Fact]
    public async void GetOriginalUrl_WhenCalled_ReturnOk()
    {
        //Arrange
        string originalUrl = "https://www.youtube.com/";
        _mediator.Send(Arg.Any<GetOriginalUrlQuery>())
            .ReturnsForAnyArgs(originalUrl);

        //Act
        var response = (await UrlEndpoints.GetOriginalUrl(_mediator, "AA", CancellationToken.None) as Ok<string>)!;
        var result = response.Value;

        //Assert
        result.Should().BeEquivalentTo(originalUrl);
    }

    [Fact]
    public async void GetOriginalUrl_WhenCalled_ReturnNotFound()
    {
        //Arrange
        _mediator.Send(Arg.Any<GetOriginalUrlQuery>())
            .ReturnsNullForAnyArgs();

        //Act
        var response = (await UrlEndpoints.GetOriginalUrl(_mediator, "AA", CancellationToken.None) as NotFound);

        //Assert
        response.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async void RedirectToOriginalUrl_WhenCalled_ReturnRedirect()
    {
        //Arrange
        string originalUrl = "https://www.youtube.com/";
        _mediator.Send(Arg.Any<GetOriginalUrlQuery>())
            .ReturnsForAnyArgs(originalUrl);

        //Act
        var response = (await UrlEndpoints.RedirectToOriginalUrl(_mediator, "AA", CancellationToken.None) as RedirectHttpResult)!;

        //Assert
        response.Should().BeOfType<RedirectHttpResult>();
    }

    [Fact]
    public async void RedirectToOriginalUrl_WhenCalled_ReturnNotFound()
    {
        //Arrange
        _mediator.Send(Arg.Any<GetOriginalUrlQuery>())
            .ReturnsNullForAnyArgs();

        //Act
        var response = (await UrlEndpoints.RedirectToOriginalUrl(_mediator, "AA", CancellationToken.None) as NotFound);

        //Assert
        response.Should().BeOfType<NotFound>();
    }

    [Fact]
    public async void ShortUrl_WhenCalled_ReturnOk()
    {
        //Arrange
        var httpContext = Substitute.For<HttpContext>();
        string originalUrl = "https://www.youtube.com/";
        string shortenedUrl = "https://localhost:7167/BA";
        _mediator.Send(Arg.Any<GetOriginalUrlQuery>())
            .ReturnsForAnyArgs(shortenedUrl);

        //Act
        var response = (await UrlEndpoints.ShortUrl(_mediator, httpContext, new ShortUrlRequest(originalUrl), CancellationToken.None) as Ok<string>)!;
        var result = response.Value;

        //Assert
        result.Should().BeEquivalentTo(shortenedUrl);
    }
}
