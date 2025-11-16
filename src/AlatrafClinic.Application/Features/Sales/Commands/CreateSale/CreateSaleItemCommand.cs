using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Commands.CreateSale;

public sealed record CreateSaleItemCommand(
    int ItemId,
    int UnitId,
    decimal Quantity,
    decimal UnitPrice
): IRequest<Result<Success>>;