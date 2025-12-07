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
using AlatrafClinic.Domain.Payments;

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
            DiagnosisType.Limbs,
            ct);

        if (diagnosisResult.IsError)
        {
            _logger.LogError("Failed to create Diagnosis for Ticket {ticketId}: {Errors}", command.TicketId, string.Join(", ", diagnosisResult.Errors));
            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;

        var incoming = new List<(int industrialPartUnitId, int quantity, decimal price)>();
        foreach (var part in command.IndustrialParts)
        {
            var partUnit = await _unitOfWork.IndustrialParts.GetByIdAndUnitId(part.IndustrialPartId, part.UnitId, ct);
            if (partUnit is null)
            {
                _logger.LogError("IndustrialPartUnit not found (PartId={PartId}, UnitId={UnitId}).", part.IndustrialPartId, part.UnitId);
                return IndustrialPartUnitErrors.IndustrialPartUnitNotFound;
            }
            
            if (part.Price != partUnit.PricePerUnit)
            {
                _logger.LogError("Price for unit is not consistant incoming {incomingPrice} and storedPrice {storedPrice}", part.Price, partUnit.PricePerUnit);
                return IndustrialPartUnitErrors.InconsistentPrice;
            }
            
            incoming.Add((partUnit.Id, part.Quantity, part.Price));
        }

        diagnosis.UpsertDiagnosisIndustrialParts(incoming);

        var repairCardResult = RepairCard.Create(diagnosis.Id, diagnosis.DiagnosisIndustrialParts.ToList(), command.Notes);

        if (repairCardResult.IsError)
        {
            _logger.LogError("Failed to create RepairCard for Ticket {ticketId}: {Errors}", command.TicketId, string.Join(", ", repairCardResult.Errors));
            return repairCardResult.Errors;
        }

        var repairCard = repairCardResult.Value;

        var paymentResult = Payment.Create(diagnosis.TicketId, diagnosis.Id, repairCard.TotalCost, PaymentReference.Repair);

        if (paymentResult.IsError)
        {
            _logger.LogError("Failed to create Payment for RepairCard : {Errors}", string.Join(", ", paymentResult.Errors));
            return paymentResult.Errors;
        }

        var payment = paymentResult.Value;

        diagnosis.AssignPayment(payment);
        diagnosis.AssignRepairCard(repairCard);
        
        await _unitOfWork.Diagnoses.AddAsync(diagnosis, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        
        _logger.LogInformation("Successfully created RepairCard {RepairCardId} for Diagnosis {DiagnosisId}.", repairCard.Id, diagnosis.Id);

        return repairCard.ToDto();
    }
}