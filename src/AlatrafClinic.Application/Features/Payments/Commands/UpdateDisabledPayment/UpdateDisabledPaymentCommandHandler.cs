
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.DisabledPayments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdateDisabledPayment;

public class UpdateDisabledPaymentCommandHandler : IRequestHandler<UpdateDisabledPaymentCommand, Result<Updated>>
{
    private readonly ILogger<UpdateDisabledPaymentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public UpdateDisabledPaymentCommandHandler(ILogger<UpdateDisabledPaymentCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<Updated>> Handle(UpdateDisabledPaymentCommand command, CancellationToken ct)
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
        var disabledPayment = await _unitOfWork.Payments.GetDisabledPaymentByPaymentIdAsync(payment.Id, ct);

        if(disabledPayment is null)
        {
            _logger.LogError("Disabled payment for payment id {PaymentId} not found", payment.Id);
            return DisabledPaymentsErrors.DisabledPaymentNotFound;
        }

        var disabledPaymentResult = disabledPayment.Update(disabledCard.Id, command.Notes);

        if (disabledPaymentResult.IsError)
        {
            _logger.LogError("Failed to update disabled payment for payment id {PaymentId}: {Error}", payment.Id, disabledPaymentResult.TopError);
            return disabledPaymentResult.Errors;
        }

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

        await _unitOfWork.Payments.UpdateDisabledPaymentAsync(disabledPayment, ct);
        await _unitOfWork.Payments.UpdateAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Successfully updated disabled payment for payment id {PaymentId}", payment.Id);
        return Result.Updated;
    }
}