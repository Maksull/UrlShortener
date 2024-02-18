using MediatR;

namespace Core.Mediatr.Queries;

public sealed record GetOriginalUrlQuery(string Code) : IRequest<string?>;
