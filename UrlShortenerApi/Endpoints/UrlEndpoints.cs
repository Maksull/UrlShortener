using Core.Contracts.UrlEndpoints;
using Core.Mediatr.Commands;
using Core.Mediatr.Queries;
using MediatR;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace UrlShortenerApi.Endpoints;

public static class UrlEndpoints
{
    public static IEndpointRouteBuilder MapUrlEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("get/{code}", GetOriginalUrl).AddFluentValidationAutoValidation().CacheOutput();
        app.MapGet("{code}", RedirectToOriginalUrl).AddFluentValidationAutoValidation();
        app.MapPost("", ShortUrl).AddFluentValidationAutoValidation();

        return app;
    }

    private static async Task<IResult> GetOriginalUrl(IMediator mediator, string code, CancellationToken cancellationToken)
    {
        var originalUrl = await mediator.Send(new GetOriginalUrlQuery(code), cancellationToken);

        if (originalUrl is not null) return Results.Ok(originalUrl);

        return Results.NotFound();
    }

    private static async Task<IResult> RedirectToOriginalUrl(IMediator mediator, string code, CancellationToken cancellationToken)
    {
        var originalUrl = await mediator.Send(new GetOriginalUrlQuery(code), cancellationToken);

        if (originalUrl is not null) return Results.Redirect(originalUrl);

        return Results.NotFound();
    }

    private static async Task<IResult> ShortUrl(IMediator mediator, HttpContext httpContext, ShortUrlRequest shortUrlRequest, CancellationToken cancellationToken)
    {
        string appUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

        var shortenedUrl = await mediator.Send(new ShortUrlCommand(appUrl, shortUrlRequest.LongUrl), cancellationToken);

        return Results.Ok(shortenedUrl);
    }
}
