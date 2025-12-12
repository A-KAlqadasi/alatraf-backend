using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Application.Features.WoundedCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.WoundedCards;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.AddWoundedCard;

public class AddWoundedCardCommandHandler : IRequestHandler<AddWoundedCardCommand, Result<WoundedCardDto>>
{
    private readonly ILogger<AddWoundedCardCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public AddWoundedCardCommandHandler(ILogger<AddWoundedCardCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<WoundedCardDto>> Handle(AddWoundedCardCommand command, CancellationToken ct)
    {
        bool exists = await _unitOfWork.WoundedCards.IsExistAsync(command.CardNumber, ct);
        if (exists)
        {
            _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
            return WoundedCardErrors.CardNumberDuplicated;
        }
        Patient? patient = await _unitOfWork.Patients.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var woundedCardResult = WoundedCard.Create(command.CardNumber, command.ExpirationDate, command.PatientId, command.CardImagePath);
        if (woundedCardResult.IsError)
        {
            return woundedCardResult.Errors;
        }

        var woundedCard = woundedCardResult.Value;
        woundedCard.Patient = patient;

        await _unitOfWork.WoundedCards.AddAsync(woundedCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Wounded card {woundedCardId} is added successfully for patient {patientId}.", woundedCard.Id, patient.Id);

        return woundedCard.ToDto();
    }
}