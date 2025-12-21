using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayment;

public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, Result<PaymentCoreDto>>
{
    private readonly ILogger<GetPaymentQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetPaymentQueryHandler(ILogger<GetPaymentQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<PaymentCoreDto>> Handle(GetPaymentQuery query, CancellationToken ct)
    {
        var payment = await _context.Payments
        .AsNoTracking()
        .FirstOrDefaultAsync(p=> p.Id == query.PaymentId, ct);
        if (payment is null)
        {
            _logger.LogWarning("Payment with id {PaymentId} was not found.", query.PaymentId);
            return PaymentErrors.PaymentNotFound;
        }

        return payment.ToBasePaymentDto();
    }
}