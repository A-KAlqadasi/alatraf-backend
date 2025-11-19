using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Cards.WoundedCards;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.WoundedPayments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Commands.UpdateWoundedPayment;

public class UpdateWoundedPaymentCommandHandler : IRequestHandler<UpdateWoundedPaymentCommand, Result<Updated>>
{
    private readonly ILogger<UpdateWoundedPaymentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public UpdateWoundedPaymentCommandHandler(ILogger<UpdateWoundedPaymentCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<Updated>> Handle(UpdateWoundedPaymentCommand command, CancellationToken ct)
    {
        var woundedCard = await _unitOfWork.Patients.GetWoundedCardByNumber(command.CardNumber, ct);

        if (woundedCard is null)
        {
            _logger.LogError("Wounded card with number {CardNumber} not found", command.CardNumber);
            return WoundedCardErrors.WoundedCardNotFound;
        }

        if(woundedCard.IsExpired)
        {
            _logger.LogError("Wounded card with number {CardNumber} is expired", command.CardNumber);
            return WoundedCardErrors.CardIsExpired;
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

        if (account.Code.ToUpper() != AlatrafClinicConstants.AccountCodes[3])
        {
            _logger.LogError("Account with id {AccountId} is not of type Wounded", command.AccountId);
            return PaymentErrors.InvalidAccountId;
        }

        var woundedPayment = await _unitOfWork.Payments.GetWoundedPaymentByIdAsync(command.PaymentId, ct);

        if (woundedPayment is null)
        {
            _logger.LogError("Wounded payment for payment id {PaymentId} not found", command.PaymentId);
            return WoundedPaymentErrors.WoundedPaymentNotFound;
        }

        var woundedPaymentResult = woundedPayment.Update(woundedCard.Id, command.TotalAmount,30000, command.ReportNumber, command.Notes);

        if (woundedPaymentResult.IsError)
        {
            _logger.LogError("Failed to update wounded payment for payment id {PaymentId}: {Error}", payment.Id, woundedPaymentResult.TopError);
            return woundedPaymentResult.Errors;
        }

        var assignResult = payment.AssignWoundedPayment(woundedPayment);

        if (assignResult.IsError)
        {
            _logger.LogError("Failed to assign wounded payment to payment id {PaymentId}: {Error}", payment.Id, assignResult.TopError);
            return assignResult.Errors;
        }

        var payResult = payment.Pay(command.TotalAmount, null, null, command.AccountId);

        if (payResult.IsError)
        {
            _logger.LogError("Failed to pay payment id {PaymentId}: {Error}", payment.Id, payResult.TopError);
            return payResult.Errors;
        }

        await _unitOfWork.Payments.UpdateWoundedPaymentAsync(woundedPayment, ct);
        await _unitOfWork.Payments.UpdateAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Wounded payment updated successfully for payment id {PaymentId}", payment.Id);
        
        return Result.Updated;
    }
}