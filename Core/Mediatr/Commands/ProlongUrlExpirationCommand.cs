using Core.Entities;
using MediatR;

namespace Core.Mediatr.Commands;

public sealed record ProlongUrlExpirationCommand(long Id, TimeSpan? ProlongFor = null) : IRequest<Url?>;
