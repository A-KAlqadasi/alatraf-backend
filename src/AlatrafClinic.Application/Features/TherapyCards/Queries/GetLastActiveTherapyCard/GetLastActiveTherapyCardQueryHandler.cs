using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetLastActiveTherapyCard;

public class GetLastActiveTherapyCardQueryHandler
    : IRequestHandler<GetLastActiveTherapyCardQuery, Result<TherapyCardDto>>
{
    private readonly ILogger<GetLastActiveTherapyCardQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetLastActiveTherapyCardQueryHandler(
        ILogger<GetLastActiveTherapyCardQueryHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TherapyCardDto>> Handle(
        GetLastActiveTherapyCardQuery query,
        CancellationToken ct)
    {

        var isExists = await _unitOfWork.Patients
            .IsExistAsync(query.PatientId, ct);
        
        if (!isExists)
        {
            _logger.LogWarning(
                "Patient with ID {PatientId} not found",
                query.PatientId);

            return PatientErrors.PatientNotFound;
        }

        var therapyCard = await _unitOfWork.TherapyCards
            .GetLastActiveTherapyCardByPatientIdAsync(query.PatientId, ct);

        if (therapyCard is null)
        {
            _logger.LogWarning(
                "No active therapy card found for patient with ID {PatientId}",
                query.PatientId);

            return TherapyCardErrors.NoActiveTherapyCardFound;
        }

        return therapyCard.ToDto();
    }
}