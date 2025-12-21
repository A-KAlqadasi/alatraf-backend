using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPatientPaymentById;

public sealed class GetPatientPaymentByPaymentIdQueryHandler
    : IRequestHandler<GetPatientPaymentByPaymentIdQuery, Result<PatientPaymentDetailsDto>>
{
    private readonly IAppDbContext _context;

    public GetPatientPaymentByPaymentIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PatientPaymentDetailsDto>> Handle(GetPatientPaymentByPaymentIdQuery query, CancellationToken ct)
    {
        var payment = await _context.Payments
            .Include(p => p.PatientPayment)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == query.PaymentId, ct);

        if (payment is null)
            return PaymentErrors.PaymentNotFound;

        if (payment.PatientPayment is null)
            return PaymentErrors.InvalidAccountKind; // or a dedicated error like PaymentErrors.PaymentTypeNotFound

        return new PatientPaymentDetailsDto
        {
            Payment = MapCore(payment),
            VoucherNumber = payment.PatientPayment.VoucherNumber,
            Notes = payment.PatientPayment.Notes
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