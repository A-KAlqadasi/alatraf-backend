using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdateDisabledPayment;

public sealed record class UpdateDisabledPaymentCommand(
    int PaymentId,
    int DiagnosisId,
    int AccountId,
    decimal TotalAmount,
    string CardNumber,
    string? Notes
) : IRequest<Result<Updated>>;