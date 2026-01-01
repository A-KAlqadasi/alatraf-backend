using AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.Enums;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;
using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;

public class UpdateRepairCardCommandHandler : IRequestHandler<UpdateRepairCardCommand, Result<Updated>>
{
     private readonly ILogger<UpdateRepairCardCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly IDiagnosisUpdateService _diagnosisUpdateService;

    public UpdateRepairCardCommandHandler(ILogger<UpdateRepairCardCommandHandler> logger, IAppDbContext context, IDiagnosisUpdateService diagnosisUpdateService)
    {
        _logger = logger;
        _context = context;
        _diagnosisUpdateService = diagnosisUpdateService;
    }
    public async Task<Result<Updated>> Handle(UpdateRepairCardCommand command, CancellationToken ct)
    {
        RepairCard? currentRepairCard = await _context.RepairCards
        .Include(r=> r.Diagnosis).ThenInclude(d=> d.DiagnosisIndustrialParts)
        .FirstOrDefaultAsync(r=> r.Id == command.RepairCardId, ct);

        if (currentRepairCard is null)
        {
            _logger.LogError("RepairCard with id {RepairCardId} not found", command.RepairCardId);

            return RepairCardErrors.RepairCardNotFound;
        }

        var isPaid = await _context.Payments.FirstOrDefaultAsync(p=> p.DiagnosisId == currentRepairCard.DiagnosisId && p.IsCompleted);
        
        if(isPaid is not null)
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
            var partUnit = await _context.IndustrialPartUnits.FirstOrDefaultAsync(i=> i.IndustrialPartId == part.IndustrialPartId && i.UnitId == part.UnitId, ct);
            if (partUnit is null)
            {
                _logger.LogError("IndustrialPartUnit not found (PartId={PartId}, UnitId={UnitId}).", part.IndustrialPartId, part.UnitId);

                return IndustrialPartUnitErrors.IndustrialPartUnitNotFound;
            }
            
            incoming.Add((partUnit.Id, part.Quantity, partUnit.PricePerUnit));
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

        var currentPayment = _context.Payments.FirstOrDefault(p=> p.DiagnosisId == currentRepairCard.DiagnosisId);

        if (currentPayment is null)
        {
            _logger.LogError("Payment for RepairCard with id {RepairCardId} not found", command.RepairCardId);
            return RepairCardErrors.PaymentNotFound;
        }

        var updatePaymentResult = currentPayment.UpdateCore(
            ticketId: updatedDiagnosis.TicketId,
            diagnosisId: updatedDiagnosis.Id,
            total: currentRepairCard.TotalCost,
            reference: PaymentReference.Repair);
        
        if (updatePaymentResult.IsError)
        {
            _logger.LogError("Failed to update payment for RepairCard with id {RepairCardId}: {Errors}", command.RepairCardId, string.Join(", ", updatePaymentResult.Errors));
            return updatePaymentResult.Errors;
        }

        updatedDiagnosis.AssignRepairCard(currentRepairCard);
        updatedDiagnosis.AssignPayment(currentPayment);

        _context.Diagnoses.Update(updatedDiagnosis);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Repair Card with id {RepairCardId} updated successfully", command.RepairCardId);
        
        return Result.Updated;
    }
}