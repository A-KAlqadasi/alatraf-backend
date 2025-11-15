using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Commands.UpdateSale;

public sealed record UpdateSaleItemCommand(
    int ItemId,
    int UnitId,
    decimal Quantity,
    decimal UnitPrice
): IRequest<Result<Success>>;