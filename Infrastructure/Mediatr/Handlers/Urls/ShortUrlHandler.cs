using Core.Entities;
using Core.Mediatr.Commands;
using HashidsNet;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        var urlInDb = await _apiDataContext.Urls.FirstOrDefaultAsync(u => u.OriginalUrl == request.LongUrl, cancellationToken);

        if (urlInDb is not null)
        {
            return urlInDb.ShortenedUrl;
        }

        var url = new Url()
        {
            OriginalUrl = request.LongUrl,
            CreatedAt = DateTime.UtcNow,
        };

        await _apiDataContext.Urls.AddAsync(url, cancellationToken);

        await _apiDataContext.SaveChangesAsync(cancellationToken);

        var code = _hashids.EncodeLong(url.Id);

        url.Code = code;
        url.ShortenedUrl = $"{request.AppUrl}/{code}";

        _apiDataContext.Urls.Update(url);

        await _apiDataContext.SaveChangesAsync(cancellationToken);

        return url.ShortenedUrl;
    }
}
