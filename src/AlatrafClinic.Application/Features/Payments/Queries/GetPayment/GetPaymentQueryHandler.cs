using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayment;

public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, Result<PaymentDto>>
{
    private readonly ILogger<GetPaymentQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetPaymentQueryHandler(ILogger<GetPaymentQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<PaymentDto>> Handle(GetPaymentQuery query, CancellationToken ct)
    {
        var payment = await _context.Payments
        .Include(p=> p.Diagnosis)
        .Include(p=> p.Ticket)
        .Include(p=> p.PatientPayment)
        .Include(p=> p.DisabledPayment)
        .Include(p=> p.WoundedPayment)
        .FirstOrDefaultAsync(p=> p.Id == query.PaymentId, ct);
        if (payment is null)
        {
            _logger.LogWarning("Payment with id {PaymentId} was not found.", query.PaymentId);
            return PaymentErrors.PaymentNotFound;
        }

        return payment.ToDto();
    }
}