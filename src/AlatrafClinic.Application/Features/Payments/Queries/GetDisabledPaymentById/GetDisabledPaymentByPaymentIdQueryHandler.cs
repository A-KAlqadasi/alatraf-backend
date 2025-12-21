using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetDisabledPaymentById;

public sealed class GetDisabledPaymentByPaymentIdQueryHandler
    : IRequestHandler<GetDisabledPaymentByPaymentIdQuery, Result<DisabledPaymentDetailsDto>>
{
    private readonly IAppDbContext _context;

    public GetDisabledPaymentByPaymentIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<DisabledPaymentDetailsDto>> Handle(GetDisabledPaymentByPaymentIdQuery query, CancellationToken ct)
    {
        var payment = await _context.Payments
            .Include(p => p.DisabledPayment)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == query.PaymentId, ct);

        if (payment is null)
            return PaymentErrors.PaymentNotFound;

        if (payment.DisabledPayment is null)
            return PaymentErrors.InvalidAccountKind;

        return new DisabledPaymentDetailsDto
        {
            Payment = MapCore(payment),
            DisabledCardId = payment.DisabledPayment.DisabledCardId,
            Notes = payment.DisabledPayment.Notes
        };
    }

    private static PaymentCoreDto MapCore(Payment p) => new()
    {
        PaymentId = p.Id,
        TicketId = p.TicketId,
        DiagnosisId = p.DiagnosisId,
        PaymentReference = p.PaymentReference,
        AccountKind = p.AccountKind,
        IsCompleted = p.IsCompleted,
        PaymentDate = p.PaymentDate,
        TotalAmount = p.TotalAmount,
        PaidAmount = p.PaidAmount,
        Discount = p.Discount,
        Residual = Math.Max(0m, p.TotalAmount - ((p.PaidAmount ?? 0m) + (p.Discount ?? 0m))),
        Notes = p.Notes
    };
}