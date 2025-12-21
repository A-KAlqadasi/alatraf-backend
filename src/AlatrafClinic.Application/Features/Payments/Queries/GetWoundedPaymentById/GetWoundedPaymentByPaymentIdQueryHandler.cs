using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetWoundedPaymentById;

public sealed class GetWoundedPaymentByPaymentIdQueryHandler
    : IRequestHandler<GetWoundedPaymentByPaymentIdQuery, Result<WoundedPaymentDetailsDto>>
{
    private readonly IAppDbContext _context;

    public GetWoundedPaymentByPaymentIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<WoundedPaymentDetailsDto>> Handle(GetWoundedPaymentByPaymentIdQuery query, CancellationToken ct)
    {
        var payment = await _context.Payments
            .Include(p => p.WoundedPayment)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == query.PaymentId, ct);

        if (payment is null)
            return PaymentErrors.PaymentNotFound;

        if (payment.WoundedPayment is null)
            return PaymentErrors.InvalidAccountKind;

        return new WoundedPaymentDetailsDto
        {
            Payment = MapCore(payment),
            WoundedCardId = payment.WoundedPayment.WoundedCardId,
            ReportNumber = payment.WoundedPayment.ReportNumber,
            Notes = payment.WoundedPayment.Notes
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