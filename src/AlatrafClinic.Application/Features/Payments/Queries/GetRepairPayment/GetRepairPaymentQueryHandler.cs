using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetRepairPayment;

public class GetRepairPaymentQueryHandler : IRequestHandler<GetRepairPaymentQuery, Result<RepairPaymentDto>>
{
    private readonly ILogger<GetRepairPaymentQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetRepairPaymentQueryHandler(ILogger<GetRepairPaymentQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<RepairPaymentDto>> Handle(GetRepairPaymentQuery query,CancellationToken ct)  
    {
        var payment = await _context.Payments
            .Include(p=> p.Diagnosis)
                .ThenInclude(p=> p.RepairCard)
            .Include(p=> p.Diagnosis)
                .ThenInclude(p=> p.DiagnosisIndustrialParts)
                    .ThenInclude(p=> p.IndustrialPartUnit)
                        .ThenInclude(p=> p.IndustrialPart)
            .Include(p=> p.Diagnosis)
                .ThenInclude(p=> p.DiagnosisIndustrialParts)
                    .ThenInclude(p=> p.IndustrialPartUnit)
                        .ThenInclude(p=> p.Unit)
            .Include(p=> p.Diagnosis)
                .ThenInclude(p=> p.Patient)
                    .ThenInclude(p=> p.Person)
            .FirstOrDefaultAsync(p=> p.Id == query.paymentId && p.PaymentReference == query.PaymentReference);

        if(payment is null)
        {
            _logger.LogError("Payment with Id {paymentId} and payment reference {paymentReference} is not found", query.paymentId, query.PaymentReference.ToString());
            return PaymentErrors.PaymentNotFound;
        }
        
        return payment.ToRepairPaymentDto();
    }
}