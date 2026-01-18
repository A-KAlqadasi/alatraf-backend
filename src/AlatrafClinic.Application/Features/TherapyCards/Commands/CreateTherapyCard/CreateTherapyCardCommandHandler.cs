using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AlatrafClinic.Domain.MedicalPrograms;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapyCard;

public sealed class CreateTherapyCardCommandHandler
    : IRequestHandler<CreateTherapyCardCommand, Result<TherapyCardDiagnosisDto>>
{
    private readonly ILogger<CreateTherapyCardCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly IDiagnosisCreationService _diagnosisService;

    public CreateTherapyCardCommandHandler(
        ILogger<CreateTherapyCardCommandHandler> logger,
        IAppDbContext context,
        IDiagnosisCreationService diagnosisService)
    {
        _logger = logger;
        _context = context;
        _diagnosisService = diagnosisService;
    }

    public async Task<Result<TherapyCardDiagnosisDto>> Handle(CreateTherapyCardCommand command, CancellationToken ct)
    {

        var diagnosisResult = await _diagnosisService.CreateAsync(
            command.TicketId,
            command.DiagnosisText,
            command.InjuryDate,
            command.InjuryReasons,
            command.InjurySides,
            command.InjuryTypes,
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
            var exists = await _context.MedicalPrograms.AnyAsync(mp=> mp.Id == program.MedicalProgramId, ct);
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
        
        var typePrice = await  _context.TherapyCardTypePrices.FirstOrDefaultAsync(x=> x.Type == command.TherapyCardType, ct);

        if (typePrice is null)
        {
            _logger.LogError("Therapy card type session price not found for type {TherapyCardType}.", command.TherapyCardType);
            return TherapyCardTypePriceErrors.InvalidPrice;
        }
        
        var price = typePrice.SessionPrice;
        DateOnly programStartDate = command.ProgramStartDate;
        DateOnly? programEndDate = command.ProgramEndDate;
        
        if(command.TherapyCardType == TherapyCardType.Special)
        {
            programStartDate= command.ProgramStartDate;
            programEndDate = null;
        }
        else
        {
            if (programEndDate == null)
            {
                _logger.LogError("Program start date and end date are required for therapy card type {TherapyCardType}.", command.TherapyCardType);
                return TherapyCardErrors.ProgramDatesAreRequired;
            }
            
            var sessions =  programEndDate.Value.DayNumber - programStartDate.DayNumber;
            if (sessions != command.NumberOfSessions)
            {
                _logger.LogError("Program dates do not match the number of sessions for therapy card type {TherapyCardType}.", command.TherapyCardType);
                return TherapyCardErrors.NumberOfSessionsInvalid;
            }
        }

        var createTherapyCardResult = TherapyCard.Create(diagnosis.Id, programStartDate, programEndDate, command.NumberOfSessions, command.TherapyCardType, price, diagnosis.DiagnosisPrograms.ToList(), TherapyCardStatus.New, null, command.Notes);

        if (createTherapyCardResult.IsError)
        {
            _logger.LogError("Failed to create TherapyCard for Diagnosis with ticket {TicketId}. Errors: {Errors}", command.TicketId, string.Join(", ", createTherapyCardResult.Errors));
           
            return createTherapyCardResult.Errors;
        }

        var therapyCard = createTherapyCardResult.Value;

        var upsertTherapyResult = therapyCard.UpsertDiagnosisPrograms(diagnosis.DiagnosisPrograms.ToList());

        if (upsertTherapyResult.IsError)
        {
            _logger.LogError("Failed to upsert therapy card programs for TherapyCard with ticket {TicketId}. Errors: {Errors}", command.TicketId, string.Join(", ", upsertTherapyResult.Errors));

            return upsertTherapyResult.Errors;
        }

        var paymentResult = Payment.Create(diagnosis.TicketId, diagnosis.Id, therapyCard.TotalCost, PaymentReference.TherapyCardNew);

        if (paymentResult.IsError)
        {
            _logger.LogError("Failed to create Payment for TherapyCard : {Errors}", string.Join(", ", paymentResult.Errors));
            return paymentResult.Errors;
        }

        var payment = paymentResult.Value;

        diagnosis.AssignTherapyCard(therapyCard);
        diagnosis.AssignPayment(payment);
        

        await _context.Diagnoses.AddAsync(diagnosis);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("TherapyCard {TherapyCardId} created for Diagnosis {DiagnosisId}.", therapyCard.Id, diagnosis.Id);
        
        return therapyCard.ToTherapyDiagnosisDto();
    }
}