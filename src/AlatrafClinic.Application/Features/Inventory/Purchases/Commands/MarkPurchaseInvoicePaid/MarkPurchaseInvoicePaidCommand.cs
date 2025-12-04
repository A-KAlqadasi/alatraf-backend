namespace AlatrafClinic.Application.Features.Inventory.Purchases.Commands.MarkPurchaseInvoicePaid;

public sealed record MarkPurchaseInvoicePaidCommand(int PurchaseInvoiceId, decimal Amount, string Method, string? Reference) : MediatR.IRequest<AlatrafClinic.Domain.Common.Results.Result<AlatrafClinic.Application.Features.Inventory.Purchases.Dtos.PurchaseInvoiceDto>>;
