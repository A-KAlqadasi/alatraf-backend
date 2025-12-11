using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;

using AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.TherapyCards.Enums;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.UpdateTherapyCard;

public class UpdateTherapyCardCommandHandler : IRequestHandler<UpdateTherapyCardCommand, Result<Updated>>
{
    private readonly ILogger<UpdateTherapyCardCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IAppDbContext _context;
    private readonly IDiagnosisUpdateService _diagnosisUpdateService;

    public UpdateTherapyCardCommandHandler(ILogger<UpdateTherapyCardCommandHandler> logger, HybridCache cache, IAppDbContext context, IDiagnosisUpdateService diagnosisUpdateService)
    {
        _logger = logger;
        _cache = cache;
        _context = context;
        _diagnosisUpdateService = diagnosisUpdateService;
    }

    public async Task<Result<Updated>> Handle(UpdateTherapyCardCommand command, CancellationToken ct)
    {
       TherapyCard? currentTherapy = await _context.TherapyCards
        .Include(d=> d.Diagnosis)
            .ThenInclude(d=> d.InjuryTypes)
        .Include(d=> d.Diagnosis)
            .ThenInclude(d=> d.InjuryReasons)
        .Include(d=> d.Diagnosis)
            .ThenInclude(d=> d.InjurySides)
        .Include(d=> d.Diagnosis)
            .ThenInclude(d=> d.DiagnosisPrograms)
        .Include(d=> d.Diagnosis)
            .ThenInclude(d=> d.Payments)
        .FirstOrDefaultAsync(th=> th.Id == command.TherapyCardId, ct);

        if (currentTherapy is null)
        {
            _logger.LogError("TherapyCard with id {TherapyCardId} not found", command.TherapyCardId);
            
            return TherapyCardErrors.TherapyCardNotFound;
        }

        if (!currentTherapy.IsEditable)
        {
            return TherapyCardErrors.Readonly;
        }

        var currentDiagnosis = currentTherapy.Diagnosis;
        if (currentDiagnosis is null)
        {
            return TherapyCardErrors.DiagnosisNotIncluded;
        }

        var updateDiagnosisResult = await _diagnosisUpdateService.UpdateAsync(currentDiagnosis.Id, command.TicketId, command.DiagnosisText, command.InjuryDate, command.InjuryReasons, command.InjurySides, command.InjuryTypes, DiagnosisType.Therapy, ct);

        if (updateDiagnosisResult.IsError)
        {
            _logger.LogError("Failed to update diagnosis for TherapyCard with id {TherapyCardId}", command.TherapyCardId);

            return updateDiagnosisResult.Errors;
        }

        var updatedDiagnosis = updateDiagnosisResult.Value;

        if (command.Programs is null || !command.Programs.Any())
        {
            return DiagnosisErrors.MedicalProgramsAreRequired;
        }

        List<(int medicalProgramId, int duration, string? notes)> diagnosisPrograms = new();

        foreach (var program in command.Programs)
        {
            var medicalProgram = await _context.MedicalPrograms.AnyAsync(mp=> mp.Id == program.MedicalProgramId, ct);
            if (!medicalProgram)
            {
                _logger.LogError("Medical program with id {MedicalProgramId} not found", program.MedicalProgramId);

                return MedicalProgramErrors.MedicalProgramNotFound;
            }
            diagnosisPrograms.Add((program.MedicalProgramId, program.Duration, program.Notes));
        }

        var upsertDiagnosisProgramsResult = updatedDiagnosis.UpsertDiagnosisPrograms(diagnosisPrograms);
        
        if (upsertDiagnosisProgramsResult.IsError)
        {
            _logger.LogError("Failed to upsert diagnosis programs for TherapyCard with id {TherapyCardId}: {Errors}", command.TherapyCardId, upsertDiagnosisProgramsResult.Errors);
            
            return upsertDiagnosisProgramsResult.Errors;
        }

        var typePrice = await _context.TherapyCardTypePrices.FirstOrDefaultAsync(t=> t.Type == command.TherapyCardType, ct);
        var price = typePrice?.SessionPrice;
        
        if (price is null)
        {
            _logger.LogError("Session price for TherapyCardType {TherapyCardType} not found", command.TherapyCardType);

            return TherapyCardTypePriceErrors.InvalidPrice;
        }

        var updateTherapyResult = currentTherapy.Update(command.ProgramStartDate, command.ProgramEndDate, command.TherapyCardType, price.Value, command.Notes);

        if (updateTherapyResult.IsError)
        {
            _logger.LogError("Failed to update TherapyCard with id {TherapyCardId}: {Errors}", command.TherapyCardId, updateTherapyResult.TopError);

            return updateTherapyResult.TopError;
        }

        var upsertTherapyResult = currentTherapy.UpsertDiagnosisPrograms(updatedDiagnosis.DiagnosisPrograms.ToList());

        if (upsertTherapyResult.IsError)
        {
            _logger.LogError("Failed to upsert diagnosis programs to TherapyCard with id {TherapyCardId}: {Errors}", command.TherapyCardId, string.Join(", ", upsertTherapyResult.Errors));

            return upsertTherapyResult.Errors;
        }

        var currentPayment = currentTherapy.Diagnosis.Payments.FirstOrDefault(t=> t.PaymentReference != PaymentReference.TherapyCardDamagedReplacement);
        
        if (currentPayment is null)
        {
            _logger.LogError("Payment for TherapyCard with id {therapyId} not found", command.TherapyCardId);
            return TherapyCardErrors.PaymentNotFound;
        }

        PaymentReference paymentReference = currentTherapy.CardStatus == TherapyCardStatus.New ? PaymentReference.TherapyCardNew : PaymentReference.TherapyCardRenew;

        var updatePaymentResult = currentPayment.UpdateCore(
            ticketId: updatedDiagnosis.TicketId,
            diagnosisId: updatedDiagnosis.Id,
            total: currentTherapy.TotalCost,
            reference: paymentReference);
        
        if (updatePaymentResult.IsError)
        {
            _logger.LogError("Failed to update payment for TherapyCard with id {therapyId}: {Errors}", command.TherapyCardId, string.Join(", ", updatePaymentResult.Errors));
            return updatePaymentResult.Errors;
        }

        updatedDiagnosis.AssignPayment(currentPayment);
        updatedDiagnosis.AssignTherapyCard(currentTherapy);

        _context.Diagnoses.Update(updatedDiagnosis);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("therapy-card", ct);

        _logger.LogInformation("TherapyCard with id {TherapyCardId} updated successfully", command.TherapyCardId);

        return Result.Updated;
    }
}