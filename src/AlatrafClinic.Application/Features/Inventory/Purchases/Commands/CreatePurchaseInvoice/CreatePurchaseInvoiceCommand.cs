using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoice;

public sealed record CreatePurchaseInvoiceCommand(
    string Number,
    DateTime Date,
    int SupplierId,
    int StoreId
) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
