using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateOrderWithItems;

public sealed record CreateOrderItemDto(int ItemId, int UnitId, decimal Quantity);

public sealed record CreateOrderWithItemsCommand(int SectionId, List<CreateOrderItemDto> Items) : IRequest<Result<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>;
