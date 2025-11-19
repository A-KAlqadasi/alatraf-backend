using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Accounts;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdatePayments;

public class UpdatePaymentCommand : IRequest<Result<Updated>>
{
    public int PaymentId { get; init; }
    public decimal? PaidAmount { get; init; }
    public decimal? Discount { get; init; }
    public AccountKind AccountKind { get; init; }
    public int? AccountId { get; init; }
    public decimal? ClientTotal { get; init; }

    public PatientPaymentDto? PatientPayment { get; init; }
    public DisabledPaymentDto? DisabledPayment { get; init; }
    public WoundedPaymentDto? WoundedPayment { get; init; }
}