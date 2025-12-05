using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.AddPurchaseItem;

public sealed record AddPurchaseItemCommand(
    int PurchaseInvoiceId,
    int StoreItemUnitId,
    decimal Quantity,
    decimal UnitPrice,
    string? Notes
) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
