using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoicesSummary;

public sealed record GetPurchaseInvoicesSummaryQuery(
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? SupplierId = null,
    int? StoreId = null
) : MediatR.IRequest<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceSummaryDto>>>, AlatrafClinic.Application.Common.Interfaces.ICachedQuery<Result<List<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceSummaryDto>>>
{
    public string CacheKey => $"purchaseinvoices:summary:from={(DateFrom?.ToString("yyyyMMdd")?? "-")}:to={(DateTo?.ToString("yyyyMMdd")?? "-")}:sup={(SupplierId?.ToString()?? "-")}:store={(StoreId?.ToString()?? "-")}";
    public string[] Tags => new[] { "purchaseinvoice", "purchase-summary" };
    public TimeSpan Expiration => TimeSpan.FromMinutes(5);
}
