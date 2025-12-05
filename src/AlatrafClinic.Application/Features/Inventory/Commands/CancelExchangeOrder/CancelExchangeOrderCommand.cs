using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Commands.CancelExchangeOrder;

public sealed record CancelExchangeOrderCommand(int ExchangeOrderId) : IRequest<Result<AlatrafClinic.Domain.Common.Results.Deleted>>;
