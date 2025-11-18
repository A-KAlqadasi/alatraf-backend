using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.CreateWoundedPayment;

public sealed record class CreateWoundedPaymentCommand(
    int PaymentId,
    int DiagnosisId,
    int AccountId,
    decimal TotalAmount,
    string CardNumber,
    string? ReportNumber = null,
    string? Notes = null) : IRequest<Result<WoundedPaymentDto>>;