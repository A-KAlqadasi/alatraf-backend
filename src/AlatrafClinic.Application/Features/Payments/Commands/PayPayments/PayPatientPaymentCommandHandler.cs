using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments.PatientPayments;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Payments.Commands.PayPayments;

public sealed record PayPatientPaymentCommand(
    int PaymentId,
    decimal PaidAmount,
    decimal? Discount,
    string VoucherNumber,
    string? Notes
) : IRequest<Result<Updated>>;


public sealed class PayPatientPaymentCommandHandler
    : IRequestHandler<PayPatientPaymentCommand, Result<Updated>>
{
    private readonly IAppDbContext _context;
    private readonly PaymentProcessor _processor;

    public PayPatientPaymentCommandHandler(IAppDbContext context, PaymentProcessor processor)
    {
        _context = context;
        _processor = processor;
    }

    public async Task<Result<Updated>> Handle(PayPatientPaymentCommand command, CancellationToken ct)
    {
        var load = await _processor.LoadForPayOnceAsync(command.PaymentId, ct);
        if (load.IsError) return load.Errors;

        var payment = load.Value;

        // Ensure voucher is unique (your existing rule)
        var voucherExists = await _context.Payments
            .Include(p => p.PatientPayment)
            .AnyAsync(p => p.PatientPayment != null && p.PatientPayment.VoucherNumber == command.VoucherNumber, ct);

        if (voucherExists)
        {
            return PatientPaymentErrors.VoucherNumberAlreadyExists;
        }

        var payResult = payment.Pay(command.PaidAmount, command.Discount);
        
        if (payResult.IsError)
        {
            return payResult.Errors;
        }

        var patientPaymentResult = PatientPayment.Create(command.VoucherNumber, payment.Id, command.Notes);

        if (patientPaymentResult.IsError)
        {
            return patientPaymentResult.Errors;
        }

        var assignResult = payment.AssignPatientPayment(patientPaymentResult.Value);
        
        if (assignResult.IsError)
        {
            return assignResult.Errors;
        } 

        return await _processor.SaveAsync(payment.Id, ct);
    }
}