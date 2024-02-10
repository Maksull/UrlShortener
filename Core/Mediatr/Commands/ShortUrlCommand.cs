using MediatR;

namespace Core.Mediatr.Commands;

public sealed record ShortUrlCommand(string AppUrl, string LongUrl) : IRequest<string>;
