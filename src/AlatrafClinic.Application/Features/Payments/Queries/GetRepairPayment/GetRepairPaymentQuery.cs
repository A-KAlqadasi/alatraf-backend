using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetRepairPayment;

public sealed record GetRepairPaymentQuery(int paymentId, PaymentReference PaymentReference) : ICachedQuery<Result<RepairPaymentDto>>
{
    public string CacheKey => $"payment:{paymentId}:ref:{PaymentReference}";
    public string[] Tags => ["payment"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(20);
}