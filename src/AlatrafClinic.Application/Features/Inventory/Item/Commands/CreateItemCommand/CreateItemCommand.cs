using MediatR;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.CreateItemCommand;

public sealed record CreateItemCommand(
    string Name,
    int BaseUnitId,
    string? Description,
    List<CreateItemUnitDto> Units
) : IRequest<Result<ItemDto>>; // نعيد Id للصنف الجديد

public sealed record CreateItemUnitDto(
    int UnitId,
    decimal Price,
    decimal ConversionFactor = 1,
    decimal? MinPriceToPay = null,
    decimal? MaxPriceToPay = null
);
