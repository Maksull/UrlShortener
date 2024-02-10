using Core.Mediatr.Commands;
using Infrastructure.BackgroundServices.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

public sealed class DeleteService : IDeleteService
{
    private readonly IMediator _mediator;
    private readonly ILogger<DeleteService> _logger;

    public DeleteService(IMediator mediator, ILogger<DeleteService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task DeleteOldUrls()
    {
        var result = await _mediator.Send(new DeleteOldUrlsCommand());

        _logger.LogInformation("Background service removed {DeletedUrlsCount} urls because of the expiration date", result.Count());
    }
}
