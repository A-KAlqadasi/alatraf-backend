using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Inventory.Purchases;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Mappers;

public static class PurchaseInvoiceMapper
{
    public static PurchaseInvoiceDto ToDto(this PurchaseInvoice invoice)
    {
        return new PurchaseInvoiceDto
        {
            Id = invoice.Id,
            Number = invoice.Number,
            Date = invoice.Date,
            SupplierId = invoice.SupplierId,
            SupplierName = invoice.Supplier?.SupplierName ?? string.Empty,
            StoreId = invoice.StoreId,
            StoreName = invoice.Store?.Name ?? string.Empty,
            Status = invoice.Status.ToString(),
            PostedAtUtc = invoice.PostedAtUtc,
            PaidAtUtc = invoice.PaidAtUtc,
            PaymentAmount = invoice.PaymentAmount,
            PaymentMethod = invoice.PaymentMethod,
            PaymentReference = invoice.PaymentReference,
            TotalQuantities = invoice.TotalQuantities,
            TotalPrice = invoice.TotalPrice
        };
    }
}
