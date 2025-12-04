using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Commands.ApproveExchangeOrder;

public sealed record ApproveExchangeOrderCommand(int ExchangeOrderId) : IRequest<Result<AlatrafClinic.Domain.Common.Results.Updated>>;
