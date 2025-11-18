using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Commands.CreateDisabledPayment;

public class CreateDisabledPaymentCommandHandler : IRequestHandler<CreateDisabledPaymentCommand, Result<DisabledPaymentDto>>
{
    private readonly ILogger<CreateDisabledPaymentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public CreateDisabledPaymentCommandHandler(ILogger<CreateDisabledPaymentCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<DisabledPaymentDto>> Handle(CreateDisabledPaymentCommand command, CancellationToken ct)
    {
        var disabledCard = await _unitOfWork.Patients.GetDisabledCardByNumber(command.CardNumber, ct);

        if (disabledCard is null)
        {
            _logger.LogError("Disabled card with number {CardNumber} not found", command.CardNumber);
            return DisabledCardErrors.DisabledCardNotFound;
        }

        if(disabledCard.IsExpired)
        {
            _logger.LogError("Disabled card with number {CardNumber} is expired", command.CardNumber);
            return DisabledCardErrors.CardIsExpired;
        }

        var payment = await _unitOfWork.Payments.GetByIdAsync(command.PaymentId, ct);

        if (payment is null)
        {
            _logger.LogError("Payment with id {PaymentId} not found", command.PaymentId);
            return PaymentErrors.PaymentNotFound;
        }

        if (payment.IsCompleted)
        {
            _logger.LogError("Payment with id {PaymentId} is already completed", command.PaymentId);
            return PaymentErrors.PaymentAlreadyCompleted;
        }

        if (payment.DiagnosisId != command.DiagnosisId)
        {
            _logger.LogError("Payment diagnosis id {PaymentDiagnosisId} does not match command diagnosis id {CommandDiagnosisId}", payment.DiagnosisId, command.DiagnosisId);
            return PaymentErrors.DiagnosisMissmatch;
        }

        var account = await _unitOfWork.Accounts.GetByIdAsync(command.AccountId, ct);
        if (account is null)
        {
            _logger.LogError("Account with id {AccountId} not found", command.AccountId);
            return PaymentErrors.InvalidAccountId;
        }

        if (account.Code.ToUpper() != AlatrafClinicConstants.AccountCodes[1])
        {
            _logger.LogError("Account with id {AccountId} is not of type Disabled", command.AccountId);
            return PaymentErrors.InvalidAccountId;
        }

        var disabledPaymentResult = DisabledPayment.Create(disabledCard.Id, payment.Id, command.Notes);

        if (disabledPaymentResult.IsError)
        {
            _logger.LogError("Failed to create disabled payment for payment id {PaymentId}: {Error}", payment.Id, disabledPaymentResult.TopError);
            return disabledPaymentResult.Errors;
        }
        var disabledPayment = disabledPaymentResult.Value;

        var assignResult = payment.AssignDisabledPayment(disabledPayment);

        if (assignResult.IsError)
        {
            _logger.LogError("Failed to assign disabled payment to payment id {PaymentId}: {Error}", payment.Id, assignResult.TopError);
            return assignResult.Errors;
        }

        var payResult = payment.Pay(command.TotalAmount, null, null, command.AccountId);

        if (payResult.IsError)
        {
            _logger.LogError("Failed to pay payment id {PaymentId}: {Error}", payment.Id, payResult.TopError);
            return payResult.Errors;
        }

        await _unitOfWork.Payments.AddDisabledPaymentAsync(disabledPayment, ct);
        await _unitOfWork.Payments.UpdateAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Successfully created disabled payment for payment id {PaymentId}", payment.Id);
        
        return payment.ToDisabledPaymentDto();
    }
}