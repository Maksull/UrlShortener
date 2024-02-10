using Core.Entities;
using Core.Mediatr.Commands;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Mediatr.Handlers.Urls;

public sealed class DeleteOldUrlsHandler : IRequestHandler<DeleteOldUrlsCommand, IEnumerable<Url>>
{
    private readonly ApiDataContext _apiDataContext;

    public DeleteOldUrlsHandler(ApiDataContext apiDataContext)
    {
        _apiDataContext = apiDataContext;
    }

    public async Task<IEnumerable<Url>> Handle(DeleteOldUrlsCommand request, CancellationToken cancellationToken)
    {
        var urls = await _apiDataContext.Urls.Where(u => u.ExpireAt <= DateTime.UtcNow).ToListAsync(cancellationToken);

        _apiDataContext.Urls.RemoveRange(urls);

        await _apiDataContext.SaveChangesAsync(cancellationToken);

        return urls;
    }
}
