using Core.Mediatr.Queries;
using HashidsNet;
using Infrastructure.Data;
using MediatR;

namespace Infrastructure.Mediatr.Handlers.Urls;

public sealed class GetOriginalUrlHandler : IRequestHandler<GetOriginalUrlQuery, string?>
{
    private readonly ApiDataContext _apiDataContext;
    private readonly IHashids _hashids;

    public GetOriginalUrlHandler(ApiDataContext apiDataContext, IHashids hashids)
    {
        _apiDataContext = apiDataContext;
        _hashids = hashids;
    }

    public async Task<string?> Handle(GetOriginalUrlQuery request, CancellationToken cancellationToken)
    {
        var urlId = _hashids.DecodeSingleLong(request.Code);

        var url = await _apiDataContext.Urls.FindAsync(urlId, cancellationToken);

        if (url is not null)
            return url.OriginalUrl;

        return null;
    }
}
