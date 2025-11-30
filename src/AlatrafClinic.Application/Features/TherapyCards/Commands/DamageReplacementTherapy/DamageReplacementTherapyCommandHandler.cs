using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.DamageReplacementTherapy;

public class DamageReplacementTherapyCommandHandler : IRequestHandler<DamageReplacementTherapyCommand, Result<Success>>
{
    private readonly ILogger<DamageReplacementTherapyCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DamageReplacementTherapyCommandHandler(ILogger<DamageReplacementTherapyCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Success>> Handle(DamageReplacementTherapyCommand command, CancellationToken ct)
    {
        var therapyCard = await _unitOfWork.TherapyCards.GetByIdAsync(command.TherapyCardId, ct);
        if (therapyCard is null)
        {
            _logger.LogWarning("Therapy card with id {TherapyCardId} not found.", command.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }

        if(therapyCard.IsExpired)
        {
            _logger.LogWarning("Therapy card with id {TherapyCardId} is expired.", command.TherapyCardId);
            return TherapyCardErrors.TherapyCardExpired;
        }
        if(!therapyCard.IsActive)
        {
            _logger.LogWarning("Therapy card with id {TherapyCardId} is not active.", command.TherapyCardId);
            return TherapyCardErrors.Readonly;
        }

        var ticket = await _unitOfWork.Tickets.GetByIdAsync(command.TicketId, ct);

        if (ticket is null)
        {
            _logger.LogWarning("Ticket with id {TicketId} not found.", command.TicketId);
            return TicketErrors.TicketNotFound;
        }

        var service = ticket.Service;

        if(service.Price is null || service.Price <= 0)
        {
            _logger.LogWarning("Service price for ticket id {TicketId} is invalid.", command.TicketId);

            return ServiceErrors.InvalidServicePrice;
        }

        var diagnosis = therapyCard.Diagnosis;

        var paymentResult = Payment.Create(command.TicketId, diagnosis.Id, service.Price.Value , PaymentReference.TherapyCardDamagedReplacement);

        if (paymentResult.IsError)
        {
            _logger.LogError("Failed to create Payment for damage replacement of TherapyCard : {TherapyCardId} Errors: {Errors}", therapyCard.Id, string.Join(", ", paymentResult.Errors));
            return paymentResult.Errors;
        }

        var payment = paymentResult.Value;

        diagnosis.AssignPayment(payment);

        await _unitOfWork.Payments.AddAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Payment {PaymentId} created for damage replacement of TherapyCard {TherapyCardId}.", payment.Id, therapyCard.Id);

        return Result.Success;
    }
}