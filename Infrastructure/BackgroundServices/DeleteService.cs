using Core.Logging.BackgroundServices;
using Core.Mediatr.Commands;
using Infrastructure.BackgroundServices.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

public sealed class DeleteService : IDeleteService
{
    private readonly IMediator _mediator;
    private readonly ILogger _logger;

    public DeleteService(IMediator mediator, ILogger<DeleteService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task DeleteOldUrls()
    {
        var result = await _mediator.Send(new DeleteOldUrlsCommand());

        _logger.LogDeletedExpiredUrl(result.Count());
    }
}
