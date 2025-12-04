using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.RemovePurchaseItem;

public sealed record RemovePurchaseItemCommand(
    int PurchaseInvoiceId,
    int StoreItemUnitId
) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
