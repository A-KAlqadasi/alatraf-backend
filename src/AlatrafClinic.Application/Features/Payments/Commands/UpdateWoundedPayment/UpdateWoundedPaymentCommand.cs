using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdateWoundedPayment;

public sealed record class UpdateWoundedPaymentCommand(
    int PaymentId,
    int DiagnosisId,
    int AccountId,
    decimal TotalAmount,
    string CardNumber,
    string? ReportNumber = null,
    string? Notes = null) : IRequest<Result<Updated>>;