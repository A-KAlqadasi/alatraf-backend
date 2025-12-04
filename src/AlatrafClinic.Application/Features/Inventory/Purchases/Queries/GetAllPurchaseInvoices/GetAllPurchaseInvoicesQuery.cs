using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetAllPurchaseInvoices;

public sealed record GetAllPurchaseInvoicesQuery() : MediatR.IRequest<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>>, AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>>
{
    public string CacheKey => "purchaseinvoices:all";
    public string[] Tags => new[] { "purchaseinvoice", "purchase-invoices" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
