using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
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
            Payment = payment.ToBasePaymentDto(),
            ReportNumber = payment.WoundedPayment.ReportNumber,
            Notes = payment.WoundedPayment.Notes
        };
    }
}