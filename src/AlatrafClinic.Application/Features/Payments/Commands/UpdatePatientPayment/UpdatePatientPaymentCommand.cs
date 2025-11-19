using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdatePatientPayment;

public sealed record class UpdatePatientPaymentCommand(
    int PaymentId,
    int DiagnosisId,
    int AccountId,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal DiscountAmount,
    string VoucherNumber,
    string? Notes
) : IRequest<Result<Updated>>;