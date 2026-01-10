using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetAllPurchaseInvoices;

public sealed record GetAllPurchaseInvoicesQuery() : MediatR.IRequest<Result<List<Dtos.PurchaseInvoiceDto>>>;
