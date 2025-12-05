using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceItems;

public sealed record GetPurchaseInvoiceItemsQuery(int PurchaseInvoiceId) : MediatR.IRequest<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseItemDto>>>, AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseItemDto>>>
{
    public string CacheKey => $"purchaseinvoice:{PurchaseInvoiceId}:items";
    public string[] Tags => new[] { "purchaseinvoice", "purchase-items" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
