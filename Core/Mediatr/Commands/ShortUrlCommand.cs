
using MediatR;

namespace Core.Mediatr.Commands;

public sealed record ShortUrlCommand(string LongUrl) : IRequest<string>;
