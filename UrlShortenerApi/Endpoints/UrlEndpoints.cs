using Core.Contracts.UrlEndpoints;
using Core.Mediatr.Commands;
using Core.Mediatr.Queries;
using MediatR;

namespace UrlShortenerApi.Endpoints;

public static class UrlEndpoints
{
    public static IEndpointRouteBuilder MapUrlEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("{shortUrl}", GetOriginalUrl);
        app.MapPost("", ShortUrl);

        return app;
    }

    private static async Task<IResult> GetOriginalUrl(IMediator mediator, string shortUrl, CancellationToken cancellationToken)
    {
        var originalUrl = await mediator.Send(new GetOriginalUrlQuery(shortUrl), cancellationToken);

        if (originalUrl is not null) return Results.Redirect(originalUrl);

        return Results.NotFound();
    }

    private static async Task<IResult> ShortUrl(IMediator mediator, ShortUrlRequest shortUrlRequest, CancellationToken cancellationToken)
    {
        var shortenedUrl = await mediator.Send(new ShortUrlCommand(shortUrlRequest.LongUrl), cancellationToken);

        return Results.Ok(shortenedUrl);
    }
}
