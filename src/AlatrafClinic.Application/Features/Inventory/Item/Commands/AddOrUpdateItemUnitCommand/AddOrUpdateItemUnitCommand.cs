using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.AddOrUpdateItemUnitCommand;
public sealed record AddOrUpdateItemUnitCommand(
    int ItemId,
    int UnitId,
    decimal Price,
    decimal ConversionFactor = 1,
    decimal? MinPriceToPay = null,
    decimal? MaxPriceToPay = null
) : IRequest<Result<Updated>>;  
