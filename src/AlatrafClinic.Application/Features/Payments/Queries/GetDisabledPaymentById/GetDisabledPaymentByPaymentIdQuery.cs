using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetDisabledPaymentById;

public sealed record GetDisabledPaymentByPaymentIdQuery(int PaymentId)
    : IRequest<Result<DisabledPaymentDetailsDto>>;
