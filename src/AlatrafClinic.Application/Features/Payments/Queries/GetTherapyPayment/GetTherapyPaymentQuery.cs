using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetTherapyPayment;

public sealed record GetTherapyPaymentQuery(int paymentId, PaymentReference PaymentReference) : IRequest<Result<TherapyPaymentDto>>;