using Core.Entities;
using MediatR;

namespace Core.Mediatr.Commands;

public sealed record DeleteOldUrlsCommand : IRequest<IEnumerable<Url>>;
