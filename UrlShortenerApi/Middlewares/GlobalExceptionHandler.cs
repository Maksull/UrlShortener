using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortenerApi.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError("An unhandled exception occurred. Exception: {exception}", exception.ToString());

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        ProblemDetails problemDetails = new()
        {
            Status = StatusCodes.Status500InternalServerError,
            Type = "Server error",
            Title = "Server error",
            Detail = "An internal server error has occurred"
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

}