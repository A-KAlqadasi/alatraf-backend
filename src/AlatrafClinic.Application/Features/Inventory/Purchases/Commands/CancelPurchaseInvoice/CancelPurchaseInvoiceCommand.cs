using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CancelPurchaseInvoice;

public sealed record CancelPurchaseInvoiceCommand(int PurchaseInvoiceId) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
