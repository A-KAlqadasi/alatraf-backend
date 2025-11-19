
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.Enums;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;

public class UpdateRepairCardCommandHandler : IRequestHandler<UpdateRepairCardCommand, Result<Updated>>
{
     private readonly ILogger<UpdateRepairCardCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiagnosisUpdateService _diagnosisUpdateService;

    public UpdateRepairCardCommandHandler(ILogger<UpdateRepairCardCommandHandler> logger, HybridCache cache, IUnitOfWork unitOfWork, IDiagnosisUpdateService diagnosisUpdateService)
    {
        _logger = logger;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _diagnosisUpdateService = diagnosisUpdateService;
    }
    public async Task<Result<Updated>> Handle(UpdateRepairCardCommand command, CancellationToken ct)
    {
        RepairCard? currentRepairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);
        if (currentRepairCard is null)
        {
            _logger.LogError("RepairCard with id {RepairCardId} not found", command.RepairCardId);

            return RepairCardErrors.RepairCardNotFound;
        }

        if (currentRepairCard.IsPaid)
        {
            _logger.LogError("RepairCard with id {RepairCardId} is readonly because it is paid", command.RepairCardId);
            
            return RepairCardErrors.Readonly;
        }

        if (currentRepairCard.Status != RepairCardStatus.New)
        {
            _logger.LogError("RepairCard with id {RepairCardId} is readonly", command.RepairCardId);
            
            return RepairCardErrors.Readonly;
        }

        var updateDiagnosisResult = await _diagnosisUpdateService.UpdateAsync(
            diagnosisId: currentRepairCard.DiagnosisId,
            ticketId: command.TicketId,
            diagnosisText: command.DiagnosisText,
            injuryDate: command.InjuryDate,
            injuryReasons: command.InjuryReasons,
            injurySides: command.InjurySides,
            injuryTypes: command.InjuryTypes,
            patientId: command.PatientId,
            DiagnosisType.Limbs,
            ct: ct);

        if (updateDiagnosisResult.IsError)
        {
            _logger.LogError("Failed to update diagnosis for RepairCard with id {RepairCardId}", command.RepairCardId);

            return updateDiagnosisResult.Errors;
        }
        
        var updatedDiagnosis = updateDiagnosisResult.Value;

        if (command.IndustrialParts is null || command.IndustrialParts.Count == 0)
        {
            return DiagnosisErrors.IndustrialPartsAreRequired;
        }

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

        var upsertDiagnosisPartsResult = updatedDiagnosis.UpsertDiagnosisIndustrialParts(incoming);

        if (upsertDiagnosisPartsResult.IsError)
        {
            _logger.LogError("Failed to upsert diagnosis programs for RepairCard with id {RepairCardId}: {Errors}", command.RepairCardId, string.Join(", ", upsertDiagnosisPartsResult.Errors));
            
            return upsertDiagnosisPartsResult.Errors;
        }

        var upsertRepairResult = currentRepairCard.UpsertIndustrialParts(updatedDiagnosis.DiagnosisIndustrialParts.ToList());

        if (upsertRepairResult.IsError)
        {
            _logger.LogError("Failed to upsert diagnosis programs to RepairCard with id {RepairCardId}: {Errors}", command.RepairCardId, string.Join(", ", upsertRepairResult.Errors));
            return upsertRepairResult.Errors;
        }

        var currentPayment = currentRepairCard.Payment;
        if (currentPayment is null)
        {
            _logger.LogError("Payment for RepairCard with id {RepairCardId} not found", command.RepairCardId);
            return RepairCardErrors.PaymentNotFound;
        }

        var updatePaymentResult = currentPayment.UpdateCore(
            diagnosisId: updatedDiagnosis.Id,
            total: currentRepairCard.TotalCost,
            reference: PaymentReference.Repair);
        
        if (updatePaymentResult.IsError)
        {
            _logger.LogError("Failed to update payment for RepairCard with id {RepairCardId}: {Errors}", command.RepairCardId, string.Join(", ", updatePaymentResult.Errors));
            return updatePaymentResult.Errors;
        }

        updatedDiagnosis.AssignPayment(currentPayment);

        await _unitOfWork.Diagnoses.UpdateAsync(updatedDiagnosis, ct);
        await _unitOfWork.RepairCards.UpdateAsync(currentRepairCard, ct);
        await _unitOfWork.Payments.UpdateAsync(currentPayment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Repair Card with id {RepairCardId} updated successfully", command.RepairCardId);
        
        return Result.Updated;
    }
}