using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.CreatePatientPayment;

public sealed record class CreatePatientPaymentCommand(
    int PaymentId,
    int DiagnosisId,
    int AccountId,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal DiscountAmount,
    string VoucherNumber,
    string? Notes
) : IRequest<Result<PatientPaymentDto>>;
