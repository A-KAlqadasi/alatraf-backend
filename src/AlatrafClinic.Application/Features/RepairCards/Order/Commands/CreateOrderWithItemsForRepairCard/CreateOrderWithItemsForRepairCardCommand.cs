using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateOrderWithItemsForRepairCard;

public sealed record CreateOrderItemForRepairCardDto(int ItemId, int UnitId, decimal Quantity);

public sealed record CreateOrderWithItemsForRepairCardCommand(int RepairCardId, int SectionId, List<CreateOrderItemForRepairCardDto> Items)
    : IRequest<Result<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>;
