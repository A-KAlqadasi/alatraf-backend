using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceItems;

public sealed record GetPurchaseInvoiceItemsQuery(int PurchaseInvoiceId)
: IRequest<Result<List<PurchaseItemDto>>>;