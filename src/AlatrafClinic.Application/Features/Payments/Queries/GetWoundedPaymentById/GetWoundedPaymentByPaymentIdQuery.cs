using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetWoundedPaymentById;

public sealed record GetWoundedPaymentByPaymentIdQuery(int PaymentId)
    : IRequest<Result<WoundedPaymentDetailsDto>>;
