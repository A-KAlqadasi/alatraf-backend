using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.WoundedCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.UpdateWoundedCard;

public class UpdateWoundedCardCommandHandler : IRequestHandler<UpdateWoundedCardCommand, Result<Updated>>
{
    private readonly ILogger<UpdateWoundedCardCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public UpdateWoundedCardCommandHandler(ILogger<UpdateWoundedCardCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<Updated>> Handle(UpdateWoundedCardCommand command, CancellationToken ct)
    {
        var currentWoundedCard = await _context.WoundedCards.FindAsync(new object[] { command.WoundedCardId }, ct);
        if (currentWoundedCard is null)
        {
            _logger.LogError("Wounded card with Id {id} not found", command.WoundedCardId);
            return WoundedCardErrors.WoundedCardNotFound;
        }
        
        if(currentWoundedCard.CardNumber.Trim() != command.CardNumber.Trim())
        {
            var isNumberExists = await _context.WoundedCards.AnyAsync(wc => wc.CardNumber == command.CardNumber);
            if (isNumberExists)
            {
                _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
                return WoundedCardErrors.CardNumberDuplicated;
            }
        }
        
        Patient? patient = await _context.Patients.FindAsync(new object[] { command.PatientId }, ct);
        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var woundedCardResult = currentWoundedCard.Update(command.CardNumber, command.IssueDate, command.ExpirationDate, command.PatientId, command.CardImagePath);
        
        if (woundedCardResult.IsError)
        {
            return woundedCardResult.Errors;
        }

        currentWoundedCard.Patient = patient;

        _context.WoundedCards.Update(currentWoundedCard);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Wounded card {cardId} is updated successfully", command.WoundedCardId);
        
        return Result.Updated;
    }
}