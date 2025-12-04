using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpsertOrderItems;

public sealed record UpsertOrderItemDto(int ItemId, int UnitId, decimal Quantity);

public sealed record UpsertOrderItemsCommand(int RepairCardId, int OrderId, List<UpsertOrderItemDto> Items) : IRequest<Result<Updated>>;
