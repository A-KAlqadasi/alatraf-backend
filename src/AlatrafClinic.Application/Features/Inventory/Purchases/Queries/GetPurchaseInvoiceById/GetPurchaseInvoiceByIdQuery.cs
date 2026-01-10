using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceById;

public sealed record GetPurchaseInvoiceByIdQuery(int PurchaseInvoiceId) : IRequest<Result<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>;