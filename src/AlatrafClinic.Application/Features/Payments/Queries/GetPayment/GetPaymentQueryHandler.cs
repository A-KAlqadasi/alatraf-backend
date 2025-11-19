using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayment;

public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, Result<PaymentDto>>
{
    private readonly ILogger<GetPaymentQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentQueryHandler(ILogger<GetPaymentQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<PaymentDto>> Handle(GetPaymentQuery query, CancellationToken ct)
    {
        var payment = await _unitOfWork.Payments.GetByIdAsync(query.PaymentId, ct);
        if (payment is null)
        {
            _logger.LogWarning("Payment with id {PaymentId} was not found.", query.PaymentId);
            return PaymentErrors.PaymentNotFound;
        }

        return payment.ToDto();
    }
}