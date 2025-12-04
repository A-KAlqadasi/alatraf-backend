using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceById;

public sealed record GetPurchaseInvoiceByIdQuery(int PurchaseInvoiceId) : ICachedQuery<Result<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>
{
    public string CacheKey => $"purchaseinvoice:id={PurchaseInvoiceId}";
    public string[] Tags => new[] { "purchaseinvoice", "purchase-invoices" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
