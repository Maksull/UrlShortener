using MediatR;

namespace Core.Mediatr.Queries;

public sealed record GetOriginalUrlQuery(string ShortUrl) : IRequest<string?>;
