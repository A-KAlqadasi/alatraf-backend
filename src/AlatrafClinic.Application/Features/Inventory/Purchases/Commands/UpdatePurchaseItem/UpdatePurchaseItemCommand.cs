using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseItem;

public sealed record UpdatePurchaseItemCommand(
    int PurchaseInvoiceId,
    int ExistingStoreItemUnitId,
    int NewStoreItemUnitId,
    decimal Quantity,
    decimal UnitPrice,
    string? Notes
) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
