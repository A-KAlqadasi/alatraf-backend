using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using MediatR;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapyCard;

public sealed class CreateTherapyCardCommandHandler
    : IRequestHandler<CreateTherapyCardCommand, Result<TherapyCardDto>>
{
    private readonly ILogger<CreateTherapyCardCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _uow;
    private readonly IDiagnosisCreationService _diagnosisService;

    public CreateTherapyCardCommandHandler(
        ILogger<CreateTherapyCardCommandHandler> logger,
        HybridCache cache,
        IUnitOfWork uow,
        IDiagnosisCreationService diagnosisService)
    {
        _logger = logger;
        _cache = cache;
        _uow = uow;
        _diagnosisService = diagnosisService;
    }

    public async Task<Result<TherapyCardDto>> Handle(CreateTherapyCardCommand command, CancellationToken ct)
    {
        if (command.Programs is null || command.Programs.Count == 0)
            return DiagnosisErrors.MedicalProgramsAreRequired;

        var diagnosisResult = await _diagnosisService.CreateAsync(
            command.TicketId,
            command.DiagnosisText,
            command.InjuryDate,
            command.InjuryReasons,
            command.InjurySides,
            command.InjuryTypes,
            command.PatientId,
            DiagnosisType.Therapy,
            ct);

        if (diagnosisResult.IsError)
        {
            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;

        // Validate programs and add to diagnosis
        foreach (var (programId, duration, notes) in command.Programs)
        {
            var exists = await _uow.MedicalPrograms.IsExistAsync(programId, ct);
            if (!exists)
            {
                _logger.LogWarning("Medical program {ProgramId} not found.", programId);
                return MedicalProgramErrors.MedicalProgramNotFound;
            }
        }

        var upsertDiagnosisResult = diagnosis.UpsertDiagnosisPrograms(command.Programs);

        if (upsertDiagnosisResult.IsError)
        {
            _logger.LogWarning("Failed to upsert diagnosis programs for Diagnosis {DiagnosisId}. Errors: {Errors}", diagnosis.Id, string.Join(", ", upsertDiagnosisResult.Errors));
            return upsertDiagnosisResult.Errors;
        }
        
        decimal? price = await _uow.TherapyCardTypePrices.GetSessionPriceByTherapyCardTypeAsync(command.TherapyCardType, ct);

        if(!price.HasValue)
        {
            _logger.LogWarning("Therapy card type session price not found for type {TherapyCardType}.", command.TherapyCardType);
            return TherapyCardTypePriceErrors.InvalidPrice;
        }

        var createTherapyCardResult = TherapyCard.Create(diagnosis.Id, command.ProgramStartDate, command.ProgramEndDate, command.TherapyCardType, price.Value, diagnosis.DiagnosisPrograms.ToList(), CardStatus.New, null, command.Notes);

        if (createTherapyCardResult.IsError)
        {
            _logger.LogWarning("Failed to create TherapyCard for Diagnosis {DiagnosisId}. Errors: {Errors}", diagnosis.Id, string.Join(", ", createTherapyCardResult.Errors));
            return createTherapyCardResult.Errors;
        }

        var therapyCard = createTherapyCardResult.Value;
        var upsertTherapyResult = therapyCard.UpsertDiagnosisPrograms(diagnosis.DiagnosisPrograms.ToList());

        if (upsertTherapyResult.IsError)
        {
            _logger.LogWarning("Failed to upsert therapy card programs for TherapyCard {TherapyCardId}. Errors: {Errors}", therapyCard.Id, string.Join(", ", upsertTherapyResult.Errors));
            return upsertTherapyResult.Errors;
        }
        

        await _uow.Diagnoses.AddAsync(diagnosis, ct);
        await _uow.TherapyCards.AddAsync(therapyCard, ct);
        await _uow.SaveChangesAsync(ct);

        // Optional cache write-through
        var dto = therapyCard.ToDto();

        //await _cache.SetAsync($"therapycard:{dto.TherapyCardId}", dto, ct: ct);

        _logger.LogInformation("TherapyCard {TherapyCardId} created for Diagnosis {DiagnosisId}.", therapyCard.Id, diagnosis.Id);
        
        return dto;
    }
}