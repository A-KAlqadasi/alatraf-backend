using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

using MediatR;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.TherapyCards.Enums;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.UpdateTherapyCard;

public class UpdateTherapyCardCommandHandler : IRequestHandler<UpdateTherapyCardCommand, Result<Updated>>
{
    private readonly ILogger<UpdateTherapyCardCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiagnosisUpdateService _diagnosisUpdateService;

    public UpdateTherapyCardCommandHandler(ILogger<UpdateTherapyCardCommandHandler> logger, HybridCache cache, IUnitOfWork unitOfWork, IDiagnosisUpdateService diagnosisUpdateService)
    {
        _logger = logger;
        _cache = cache;
        _unitOfWork = unitOfWork;
        _diagnosisUpdateService = diagnosisUpdateService;
    }

    public async Task<Result<Updated>> Handle(UpdateTherapyCardCommand command, CancellationToken ct)
    {
        TherapyCard? currentTherapy = await _unitOfWork.TherapyCards.GetByIdAsync(command.TherapyCardId, ct);
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

        var updateDiagnosisResult = await _diagnosisUpdateService.UpdateAsync(currentDiagnosis.Id, command.TicketId, command.DiagnosisText, command.InjuryDate, command.InjuryReasons, command.InjurySides, command.InjuryTypes, command.PatientId, DiagnosisType.Therapy, ct);

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
            var medicalProgram = await _unitOfWork.MedicalPrograms.IsExistAsync(program.MedicalProgramId, ct);
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

        var sessionPricePerType = await _unitOfWork.TherapyCardTypePrices.GetSessionPriceByTherapyCardTypeAsync(command.TherapyCardType, ct);
        
        if (!sessionPricePerType.HasValue)
        {
            _logger.LogError("Session price for TherapyCardType {TherapyCardType} not found", command.TherapyCardType);

            return TherapyCardTypePriceErrors.InvalidPrice;
        }

        var updateTherapyResult = currentTherapy.Update(command.ProgramStartDate, command.ProgramEndDate, command.TherapyCardType, sessionPricePerType.Value, command.Notes);

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

        var currentPayment = currentTherapy.Payment;
        
        if (currentPayment is null)
        {
            _logger.LogError("Payment for TherapyCard with id {therapyId} not found", command.TherapyCardId);
            return TherapyCardErrors.PaymentNotFound;
        }

        PaymentType paymentType = currentTherapy.CardStatus == CardStatus.New ? PaymentType.NewTherapyCard : PaymentType.RenewTherapyCard;

        var updatePaymentResult = currentPayment.Update(
            diagnosisId: updatedDiagnosis.Id,
            total: currentTherapy.TotalCost,
            type: paymentType);
        
        if (updatePaymentResult.IsError)
        {
            _logger.LogError("Failed to update payment for TherapyCard with id {therapyId}: {Errors}", command.TherapyCardId, string.Join(", ", updatePaymentResult.Errors));
            return updatePaymentResult.Errors;
        }

        updatedDiagnosis.AssignPayment(currentPayment);

        await _unitOfWork.Diagnoses.UpdateAsync(updatedDiagnosis, ct);
        await _unitOfWork.TherapyCards.UpdateAsync(currentTherapy, ct);
        await _unitOfWork.Payments.UpdateAsync(currentPayment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("TherapyCard with id {TherapyCardId} updated successfully", command.TherapyCardId);

        return Result.Updated;
    }
}