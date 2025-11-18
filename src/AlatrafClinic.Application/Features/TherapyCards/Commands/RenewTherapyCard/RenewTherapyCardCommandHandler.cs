
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.RenewTherapyCard;

public class RenewTherapyCardCommandHandler : IRequestHandler<RenewTherapyCardCommand, Result<TherapyCardDto>>
{
    private readonly ILogger<RenewTherapyCardCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;
    private readonly IDiagnosisCreationService _diagnosisService;

    public RenewTherapyCardCommandHandler(ILogger<RenewTherapyCardCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache, IDiagnosisCreationService diagnosisService)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _diagnosisService = diagnosisService;
    }

    public async Task<Result<TherapyCardDto>> Handle(RenewTherapyCardCommand command, CancellationToken ct)
    {
        TherapyCard? currentTherapy = await _unitOfWork.TherapyCards.GetByIdAsync(command.TherapyCardId, ct);

        if (currentTherapy is null)
        {
            _logger.LogError("TherapyCard with id {TherapyCardId} not found", command.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }
        if (!currentTherapy.IsExpired)
        {
            _logger.LogError("Therapy Card {TherapyCardId} is not expired to renew", currentTherapy.Id);
            return TherapyCardErrors.TherapyCardNotExpired;
        }

        var currentDiagnosis = currentTherapy.Diagnosis;

        if (currentDiagnosis is null)
        {
            _logger.LogError("Diagnosis for therapy card {therapyCard} not included", currentTherapy.Id);

            return TherapyCardErrors.DiagnosisNotIncluded;
        }
        
        if (command.Programs is null || command.Programs.Count == 0)
            return DiagnosisErrors.MedicalProgramsAreRequired;

        var diagnosisResult = await _diagnosisService.CreateAsync(
            command.TicketId,
            currentDiagnosis.DiagnosisText,
            currentDiagnosis.InjuryDate,
            currentDiagnosis.InjuryReasons.Select(r=> r.Id).ToList(),
            currentDiagnosis.InjurySides.Select(s=> s.Id).ToList(),
            currentDiagnosis.InjuryTypes.Select(s=> s.Id).ToList(),
            currentDiagnosis.PatientId,
            DiagnosisType.Therapy,
            ct);

        if (diagnosisResult.IsError)
        {
            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;
        
        List<(int medicalProgramId, int duration, string? notes)> diagnosisPrograms = new();
        foreach (var program in command.Programs)
        {
            var exists = await _unitOfWork.MedicalPrograms.IsExistAsync(program.MedicalProgramId, ct);
            if (!exists)
            {
                _logger.LogError("Medical program {ProgramId} not found.", program.MedicalProgramId);

                return MedicalProgramErrors.MedicalProgramNotFound;
            }
            diagnosisPrograms.Add((program.MedicalProgramId, program.Duration, program.Notes));
        }

        var upsertDiagnosisResult = diagnosis.UpsertDiagnosisPrograms(diagnosisPrograms);

        if (upsertDiagnosisResult.IsError)
        {
            _logger.LogError("Failed to upsert diagnosis programs for Diagnosis with ticket {TicketId}. Errors: {Errors}", command.TicketId, string.Join(", ", upsertDiagnosisResult.Errors));

            return upsertDiagnosisResult.Errors;
        }
        
        decimal? price = await _unitOfWork.TherapyCardTypePrices.GetSessionPriceByTherapyCardTypeAsync(command.TherapyCardType, ct);

        if(!price.HasValue)
        {
            _logger.LogError("Therapy card type session price not found for type {TherapyCardType}.", command.TherapyCardType);

            return TherapyCardTypePriceErrors.InvalidPrice;
        }

        var createTherapyCardResult = TherapyCard.Create(diagnosis.Id, command.ProgramStartDate, command.ProgramEndDate, command.TherapyCardType, price.Value, diagnosis.DiagnosisPrograms.ToList(), CardStatus.Renew, currentTherapy.Id, command.Notes);

        if (createTherapyCardResult.IsError)
        {
            _logger.LogError("Failed to create TherapyCard for Diagnosis with ticket {ticketId}. Errors: {Errors}", command.TicketId, string.Join(", ", createTherapyCardResult.Errors));
            
            return createTherapyCardResult.Errors;
        }

        var therapyCard = createTherapyCardResult.Value;
        var upsertTherapyResult = therapyCard.UpsertDiagnosisPrograms(diagnosis.DiagnosisPrograms.ToList());

        if (upsertTherapyResult.IsError)
        {
            _logger.LogError("Failed to upsert therapy card programs for TherapyCard with ticket {TicketId}. Errors: {Errors}", command.TicketId, string.Join(", ", upsertTherapyResult.Errors));

            return upsertTherapyResult.Errors;
        }
        
         var paymentResult = Payment.Create(diagnosis.Id, therapyCard.TotalCost, PaymentType.RenewTherapyCard);

        if (paymentResult.IsError)
        {
            _logger.LogError("Failed to create Payment for TherapyCard : {Errors}", string.Join(", ", paymentResult.Errors));
            return paymentResult.Errors;
        }

        var payment = paymentResult.Value;

        diagnosis.AssignPayment(payment);
        
        await _unitOfWork.Diagnoses.AddAsync(diagnosis, ct);
        await _unitOfWork.TherapyCards.AddAsync(therapyCard, ct);
        await _unitOfWork.Payments.AddAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("TherapyCard {CurrentTherapyCard} Renewed with {NewTherapyCard} for Diagnosis {DiagnosisId}.", command.TherapyCardId, therapyCard.Id, diagnosis.Id);
        
        return therapyCard.ToDto();;
    }
}