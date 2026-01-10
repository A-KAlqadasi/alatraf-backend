using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoicesSummary;

public sealed record GetPurchaseInvoicesSummaryQuery(
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? SupplierId = null,
    int? StoreId = null
) : MediatR.IRequest<Result<List<PurchaseInvoiceSummaryDto>>>;
