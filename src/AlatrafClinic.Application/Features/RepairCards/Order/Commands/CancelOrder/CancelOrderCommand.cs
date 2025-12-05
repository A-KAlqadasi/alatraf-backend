using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CancelOrder;

public sealed record CancelOrderCommand(int OrderId) : IRequest<Result<Updated>>;
