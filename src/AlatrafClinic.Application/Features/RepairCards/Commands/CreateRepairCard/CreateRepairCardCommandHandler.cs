using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;


namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCard;

public sealed class CreateRepairCardCommandHandler
    : IRequestHandler<CreateRepairCardCommand, Result<RepairCardDto>>
{
    private readonly ILogger<CreateRepairCardCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiagnosisCreationService _diagnosisService;

    public CreateRepairCardCommandHandler(
        ILogger<CreateRepairCardCommandHandler> logger,
        HybridCache cache,
        IUnitOfWork unitOfWork,
        IDiagnosisCreationService diagnosisService)
    {
        _logger = logger;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _diagnosisService = diagnosisService;
    }

    public async Task<Result<RepairCardDto>> Handle(CreateRepairCardCommand command, CancellationToken ct)
    {
        if (command.IndustrialParts is null || command.IndustrialParts.Count == 0)
        {
            return DiagnosisErrors.IndustrialPartsAreRequired;
        }

        var diagnosisResult = await _diagnosisService.CreateAsync(
            command.TicketId,
            command.DiagnosisText,
            command.InjuryDate,
            command.InjuryReasons,
            command.InjurySides,
            command.InjuryTypes,
            command.PatientId,
            DiagnosisType.Limbs,
            ct);

        if (diagnosisResult.IsError)
        {
            _logger.LogWarning("Failed to create Diagnosis for Ticket {ticketId}: {Errors}",command.TicketId, diagnosisResult.Errors);
            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;

        var incoming = new List<(int industrialPartUnitId, int quantity, decimal price)>();
        foreach (var (partId, unitId, quantity, price) in command.IndustrialParts)
        {
            var partUnit = await _unitOfWork.IndustrialParts.GetByIdAndUnitId(partId, unitId, ct);
            if (partUnit is null)
            {
                _logger.LogWarning("IndustrialPartUnit not found (PartId={PartId}, UnitId={UnitId}).", partId, unitId);
                return IndustrialPartUnitErrors.IndustrialPartUnitNotFound;
            }
            
            if (price != partUnit.PricePerUnit)
            {
                _logger.LogWarning("Price for unit is not consistant incoming {incomingPrice} and storedPrice {storedPrice}", price, partUnit.PricePerUnit);
                return IndustrialPartUnitErrors.InconsistentPrice;
            }
            
            incoming.Add((partUnit.Id, quantity, price));
        }

        diagnosis.UpsertDiagnosisIndustrialParts(incoming);

        var repairCardResult = RepairCard.Create(diagnosis.Id, diagnosis.DiagnosisIndustrialParts.ToList(), command.Notes);

        if (repairCardResult.IsError)
        {
            _logger.LogWarning("Failed to create RepairCard for Ticket {ticketId}: {Errors}", command.TicketId, string.Join(", ", repairCardResult.Errors));
            return repairCardResult.Errors;
        }

        var repairCard = repairCardResult.Value;

        await _unitOfWork.Diagnoses.AddAsync(diagnosis, ct);
        await _unitOfWork.RepairCards.AddAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        //await _cache.SetAsync($"repaircard:{dto.RepairCardId}", dto, ct: ct);

        _logger.LogInformation("Successfully created RepairCard {RepairCardId} for Diagnosis {DiagnosisId}.", repairCard.Id, diagnosis.Id);

        return repairCard.ToDto();
    }
}