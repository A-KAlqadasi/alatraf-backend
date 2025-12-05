using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

using static AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoiceWithItems.CreatePurchaseInvoiceWithItemsCommand;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoiceWithItems;

public sealed record CreatePurchaseInvoiceWithItemsCommand(
    string Number,
    DateTime Date,
    int SupplierId,
    int StoreId,
    IEnumerable<CreatePurchaseInvoiceItem> Items
) : MediatR.IRequest<Result<PurchaseInvoiceDto>>
{
    public sealed record CreatePurchaseInvoiceItem(int StoreItemUnitId, decimal Quantity, decimal UnitPrice, string? Notes);
}
