using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.UpdateDisabledCard;

public class UpdateDisabledCardCommandHandler : IRequestHandler<UpdateDisabledCardCommand, Result<Updated>>
{
    private readonly ILogger<UpdateDisabledCardCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public UpdateDisabledCardCommandHandler(ILogger<UpdateDisabledCardCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<Updated>> Handle(UpdateDisabledCardCommand command, CancellationToken ct)
    {

        var currentDisabledCard = await _context.DisabledCards.FirstOrDefaultAsync(c=> c.Id == command.DisabledCardId, ct);

        if (currentDisabledCard is null)
        {
            _logger.LogError("Disabled card with Id {id} not found", command.DisabledCardId);
            return DisabledCardErrors.DisabledCardNotFound;
        }
        
        if(currentDisabledCard.CardNumber.Trim() != command.CardNumber.Trim())
        {
            var isNumberExists = await _context.DisabledCards.AnyAsync(c=> c.CardNumber == command.CardNumber, ct);
            if (isNumberExists)
            {
                _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
                return DisabledCardErrors.CardNumberDuplicated;
            }
        }
        
        Patient? patient = await _context.Patients.FirstOrDefaultAsync(p=> p.Id == command.PatientId, ct);

        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var disabledCardResult = currentDisabledCard.Update(command.CardNumber, command.IssueDate, command.ExpirationDate, command.PatientId, command.CardImagePath);
        
        if (disabledCardResult.IsError)
        {
            return disabledCardResult.Errors;
        }

        currentDisabledCard.Patient = patient;

        _context.DisabledCards.Update(currentDisabledCard);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Disabled card {cardId} is updated successfully", command.DisabledCardId);
        
        return Result.Updated;
    }
}