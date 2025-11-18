using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.CreateDisabledPayment;

public sealed record class CreateDisabledPaymentCommand(
    int PaymentId,
    int DiagnosisId,
    int AccountId,
    decimal TotalAmount,
    string CardNumber,
    string? Notes
) : IRequest<Result<DisabledPaymentDto>>;