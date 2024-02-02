using Core.Entities;
using Core.Mediatr.Commands;
using HashidsNet;
using Infrastructure.Data;
using MediatR;

namespace Infrastructure.Mediatr.Handlers.Urls;

public sealed class ShortUrlHandler : IRequestHandler<ShortUrlCommand, string>
{
    private readonly ApiDataContext _apiDataContext;
    private readonly IHashids _hashids;

    public ShortUrlHandler(ApiDataContext apiDataContext, IHashids hashids)
    {
        _apiDataContext = apiDataContext;
        _hashids = hashids;
    }

    public async Task<string> Handle(ShortUrlCommand request, CancellationToken cancellationToken)
    {
        var url = new Url()
        {
            OriginalUrl = request.LongUrl,
            CreatedAt = DateTime.UtcNow,
        };

        await _apiDataContext.Urls.AddAsync(url, cancellationToken);

        await _apiDataContext.SaveChangesAsync(cancellationToken);

        url.ShortenedUrl = _hashids.EncodeLong(url.Id);

        _apiDataContext.Urls.Update(url);

        await _apiDataContext.SaveChangesAsync(cancellationToken);

        return url.ShortenedUrl;
    }
}
