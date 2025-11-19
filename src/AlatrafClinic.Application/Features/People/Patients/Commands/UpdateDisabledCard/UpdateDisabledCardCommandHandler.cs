
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Cards.DisabledCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdateDisabledCard;

public class UpdateDisabledCardCommandHandler : IRequestHandler<UpdateDisabledCardCommand, Result<Updated>>
{
    private readonly ILogger<UpdateDisabledCardCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public UpdateDisabledCardCommandHandler(ILogger<UpdateDisabledCardCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateDisabledCardCommand command, CancellationToken ct)
    {

        var currentDisabledCard = await _unitOfWork.Patients.GetDisabledCardByIdAsync(command.DisabledCardId, ct);
        if (currentDisabledCard is null)
        {
            _logger.LogError("Disabled card with Id {id} not found", command.DisabledCardId);
            return DisabledCardErrors.DisabledCardNotFound;
        }
        
        if(currentDisabledCard.CardNumber.Trim() != command.CardNumber.Trim())
        {
            var isNumberExists = await _unitOfWork.Patients.IsDisabledCardExists(command.CardNumber);
            if (isNumberExists)
            {
                _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
                return DisabledCardErrors.CardNumberDuplicated;
            }
        }
        
        Patient? patient = await _unitOfWork.Patients.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var disabledCardResult = currentDisabledCard.Update(command.CardNumber, command.ExpirationDate, command.PatientId, command.CardImagePath);
        if (disabledCardResult.IsError)
        {
            return disabledCardResult.Errors;
        }

        currentDisabledCard.Patient = patient;

        await _unitOfWork.Patients.UpdateDisabledCardAsync(currentDisabledCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Disabled card {cardId} is updated successfully", command.DisabledCardId);
        
        return Result.Updated;
    }
}