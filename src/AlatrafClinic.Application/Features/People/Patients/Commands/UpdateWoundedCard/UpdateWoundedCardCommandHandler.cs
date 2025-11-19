
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.Patients.Cards.WoundedCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdateWoundedCard;

public class UpdateWoundedCardCommandHandler : IRequestHandler<UpdateWoundedCardCommand, Result<Updated>>
{
    private readonly ILogger<UpdateWoundedCardCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public UpdateWoundedCardCommandHandler(ILogger<UpdateWoundedCardCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }

    public async Task<Result<Updated>> Handle(UpdateWoundedCardCommand command, CancellationToken ct)
    {
        var currentWoundedCard = await _unitOfWork.Patients.GetWoundedCardByIdAsync(command.WoundedCardId, ct);
        if (currentWoundedCard is null)
        {
            _logger.LogError("Wounded card with Id {id} not found", command.WoundedCardId);
            return WoundedCardErrors.WoundedCardNotFound;
        }
        
        if(currentWoundedCard.CardNumber.Trim() != command.CardNumber.Trim())
        {
            var isNumberExists = await _unitOfWork.Patients.IsWoundedCardExists(command.CardNumber);
            if (isNumberExists)
            {
                _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
                return WoundedCardErrors.CardNumberDuplicated;
            }
        }
        
        Patient? patient = await _unitOfWork.Patients.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var woundedCardResult = currentWoundedCard.Update(command.CardNumber, command.ExpirationDate, command.PatientId, command.CardImagePath);
        if (woundedCardResult.IsError)
        {
            return woundedCardResult.Errors;
        }

        currentWoundedCard.Patient = patient;

        await _unitOfWork.Patients.UpdateWoundedCardAsync(currentWoundedCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Wounded card {cardId} is updated successfully", command.WoundedCardId);
        
        return Result.Updated;
    }
}