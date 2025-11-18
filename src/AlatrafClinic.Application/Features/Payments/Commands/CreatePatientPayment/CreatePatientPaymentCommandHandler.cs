
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Payments.PatientPayments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Commands.CreatePatientPayment;

public class CreatePatientPaymentCommandHandler : IRequestHandler<CreatePatientPaymentCommand, Result<PatientPaymentDto>>
{
    private readonly ILogger<CreatePatientPaymentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public CreatePatientPaymentCommandHandler(ILogger<CreatePatientPaymentCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<PatientPaymentDto>> Handle(CreatePatientPaymentCommand command, CancellationToken ct)
    {
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

        if (account.Code.ToUpper() != AlatrafClinicConstants.AccountCodes[0])
        {
            _logger.LogError("Account with id {AccountId} is not of type Patient", command.AccountId);
            return PaymentErrors.InvalidAccountId;
        }
        var patientPaymentResult = PatientPayment.Create(command.VoucherNumber, payment.Id, command.Notes);

        if (patientPaymentResult.IsError)
        {
            _logger.LogError("Failed to create patient payment for payment id {PaymentId}: {Error}", payment.Id, patientPaymentResult.TopError);
            return patientPaymentResult.Errors;
        }
        var patientPayment = patientPaymentResult.Value;

        var assignResult = payment.AssignPatientPayment(patientPayment);

        if (assignResult.IsError)
        {
            _logger.LogError("Failed to assign patient payment to payment id {PaymentId}: {Error}", payment.Id, assignResult.TopError);
            return assignResult.Errors;
        }
        if (command.TotalAmount != payment.TotalAmount)
        {
            _logger.LogError("Total amount missmatch for payment id {PaymentId}: command total {CommandTotal} does not match payment total {PaymentTotal}", payment.Id, command.TotalAmount, payment.TotalAmount);
            return PaymentErrors.TotalMissmatch;
        }
        if(command.TotalAmount > (command.PaidAmount + command.DiscountAmount))
        {
            _logger.LogError("Paid amount and discount are less than total for payment id {PaymentId}: total {Total} paid {Paid} discount {Discount}", payment.Id, command.TotalAmount, command.PaidAmount, command.DiscountAmount);
            return PaymentErrors.PaidAmountLessThanTotal;
        }

        var payResult = payment.Pay(command.TotalAmount, command.PaidAmount, command.DiscountAmount, command.AccountId);

        if (payResult.IsError)
        {
            _logger.LogError("Failed to pay payment id {PaymentId}: {Error}", payment.Id, payResult.TopError);
            return payResult.Errors;
        }
        await _unitOfWork.Payments.AddPatientPaymentAsync(patientPayment, ct);
        await _unitOfWork.Payments.UpdateAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Successfully created patient payment for payment id {PaymentId}", payment.Id);

        return payment.ToPatientPaymentDto();
    }
}