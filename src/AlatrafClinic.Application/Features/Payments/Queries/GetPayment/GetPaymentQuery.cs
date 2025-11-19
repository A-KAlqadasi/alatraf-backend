using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayment;

public sealed record GetPaymentQuery(int PaymentId) : IRequest<Result<PaymentDto>>;