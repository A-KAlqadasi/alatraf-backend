using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Application.Features.DisabledCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.AddDisabledCard;

public class AddDisabledCardCommandHandler : IRequestHandler<AddDisabledCardCommand, Result<DisabledCardDto>>
{
    private readonly ILogger<AddDisabledCardCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cache;

    public AddDisabledCardCommandHandler(ILogger<AddDisabledCardCommandHandler> logger, IUnitOfWork unitOfWork, ICacheService cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<DisabledCardDto>> Handle(AddDisabledCardCommand command, CancellationToken ct)
    {
        bool exists = await _unitOfWork.DisabledCards.IsExistAsync(command.CardNumber, ct);
        if (exists)
        {
            _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
            return DisabledCardErrors.CardNumberDuplicated;
        }
        Patient? patient = await _unitOfWork.Patients.GetByIdAsync(command.PatientId, ct);
        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var disabledCardResult = DisabledCard.Create(command.CardNumber, command.ExpirationDate, command.PatientId, command.CardImagePath);
        if (disabledCardResult.IsError)
        {
            return disabledCardResult.Errors;
        }
        var disabledCard = disabledCardResult.Value;
        disabledCard.Patient = patient;

        await _unitOfWork.DisabledCards.AddAsync(disabledCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return disabledCard.ToDto();
    }
}