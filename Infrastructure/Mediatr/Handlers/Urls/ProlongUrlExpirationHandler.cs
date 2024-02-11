using Core.Entities;
using Core.Mediatr.Commands;
using Infrastructure.Data;
using MediatR;

namespace Infrastructure.Mediatr.Handlers.Urls;

public sealed class ProlongUrlExpirationHandler : IRequestHandler<ProlongUrlExpirationCommand, Url?>
{
    private readonly ApiDataContext _apiDataContext;

    public ProlongUrlExpirationHandler(ApiDataContext apiDataContext)
    {
        _apiDataContext = apiDataContext;
    }

    public async Task<Url?> Handle(ProlongUrlExpirationCommand request, CancellationToken cancellationToken)
    {
        var url = await _apiDataContext.Urls.FindAsync(request.Id, cancellationToken);

        if (url is null)
            return url;

        url.ExpireAt = DateTime.UtcNow.Add(request.ProlongFor ?? TimeSpan.FromMinutes(5));

        _apiDataContext.Urls.Update(url);

        await _apiDataContext.SaveChangesAsync(cancellationToken);

        return url;
    }
}
