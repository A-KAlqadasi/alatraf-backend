using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseInvoice;

public sealed record UpdatePurchaseInvoiceCommand(
    int Id,
    string Number,
    DateTime Date,
    int SupplierId,
    int StoreId
) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
