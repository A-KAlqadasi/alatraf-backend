using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
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
            Payment = payment.ToBasePaymentDto(),
            DisabledCardId = payment.DisabledPayment.DisabledCardId,
            Notes = payment.DisabledPayment.Notes
        };
    }
}