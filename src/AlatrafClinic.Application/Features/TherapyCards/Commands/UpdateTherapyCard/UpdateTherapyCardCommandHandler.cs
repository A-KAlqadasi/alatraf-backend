
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.UpdateTherapyCard;

public class UpdateTherapyCardCommandHandler : IRequestHandler<UpdateTherapyCardCommand, Result<Updated>>
{
    private readonly ILogger<UpdateTherapyCardCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _uow;
    private readonly IDiagnosisUpdateService _diagnosisUpdateService;

    public UpdateTherapyCardCommandHandler(ILogger<UpdateTherapyCardCommandHandler> logger, HybridCache cache, IUnitOfWork uow, IDiagnosisUpdateService diagnosisUpdateService)
    {
        _logger = logger;
        _cache = cache;
        _uow = uow;
        _diagnosisUpdateService = diagnosisUpdateService;
    }

    public async Task<Result<Updated>> Handle(UpdateTherapyCardCommand command, CancellationToken ct)
    {
        TherapyCard? currentTherapy = await _uow.TherapyCards.GetByIdAsync(command.TherapyCardId, ct);
        if (currentTherapy is null)
        {
            _logger.LogWarning("TherapyCard with id {TherapyCardId} not found", command.TherapyCardId);
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
            _logger.LogWarning("Failed to update diagnosis for TherapyCard with id {TherapyCardId}", command.TherapyCardId);
            return updateDiagnosisResult.Errors;
        }

        var updatedDiagnosis = updateDiagnosisResult.Value;

        if (command.Programs is null || !command.Programs.Any())
        {
            return DiagnosisErrors.MedicalProgramsAreRequired;
        }

        foreach (var (medicalProgramId, duration, notes) in command.Programs)
        {
            var medicalProgram = await _uow.MedicalPrograms.IsExistAsync(medicalProgramId, ct);
            if (!medicalProgram)
            {
                _logger.LogWarning("Medical program with id {MedicalProgramId} not found", medicalProgramId);

                return MedicalProgramErrors.MedicalProgramNotFound;
            }
        }

        var upsertDiagnosisProgramsResult = updatedDiagnosis.UpsertDiagnosisPrograms(command.Programs, command.TherapyCardId);
        
        if (upsertDiagnosisProgramsResult.IsError)
        {
            _logger.LogWarning("Failed to upsert diagnosis programs for TherapyCard with id {TherapyCardId}: {Errors}", command.TherapyCardId, upsertDiagnosisProgramsResult.Errors);
            return upsertDiagnosisProgramsResult.Errors;
        }

        var sessionPricePerType = await _uow.TherapyCardTypePrices.GetSessionPriceByTherapyCardTypeAsync(command.TherapyCardType, ct);
        if (!sessionPricePerType.HasValue)
        {
            _logger.LogWarning("Session price for TherapyCardType {TherapyCardType} not found", command.TherapyCardType);
            return TherapyCardTypePriceErrors.InvalidPrice;
        }

        var updateTherapyResult = currentTherapy.Update(command.ProgramStartDate, command.ProgramEndDate, command.TherapyCardType, sessionPricePerType.Value, command.Notes);

        if (updateTherapyResult.IsError)
        {
            _logger.LogWarning("Failed to update TherapyCard with id {TherapyCardId}: {Errors}", command.TherapyCardId, updateTherapyResult.TopError);
            return updateTherapyResult.TopError;
        }

        var upsertTherapyResult = currentTherapy.UpsertDiagnosisPrograms(updatedDiagnosis.DiagnosisPrograms.ToList());

        if (upsertTherapyResult.IsError)
        {
            _logger.LogWarning("Failed to upsert diagnosis programs to TherapyCard with id {TherapyCardId}: {Errors}", command.TherapyCardId, upsertTherapyResult.Errors);
            return upsertTherapyResult.Errors;
        }

        await _uow.Diagnoses.UpdateAsync(updatedDiagnosis, ct);
        await _uow.TherapyCards.UpdateAsync(currentTherapy, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("TherapyCard with id {TherapyCardId} updated successfully", command.TherapyCardId);

        return Result.Updated;
    }
}