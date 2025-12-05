using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.ApprovePurchaseInvoice;

public sealed record ApprovePurchaseInvoiceCommand(int PurchaseInvoiceId) : MediatR.IRequest<Result<PurchaseInvoiceDto>>;
