using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.SubmitPurchaseInvoice;

public sealed record SubmitPurchaseInvoiceCommand(int PurchaseInvoiceId) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
