using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetRepairPayment;

public sealed record GetRepairPaymentQuery(int paymentId, PaymentReference PaymentReference) : IRequest<Result<RepairPaymentDto>>;