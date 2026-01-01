using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPatientPaymentById;

public sealed record GetPatientPaymentByPaymentIdQuery(int PaymentId)
    : IRequest<Result<PatientPaymentDetailsDto>>;
