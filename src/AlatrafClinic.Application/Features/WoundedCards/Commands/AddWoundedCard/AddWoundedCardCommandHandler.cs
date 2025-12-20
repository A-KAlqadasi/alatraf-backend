using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Application.Features.WoundedCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;
using AlatrafClinic.Domain.WoundedCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.AddWoundedCard;

public class AddWoundedCardCommandHandler : IRequestHandler<AddWoundedCardCommand, Result<WoundedCardDto>>
{
    private readonly ILogger<AddWoundedCardCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public AddWoundedCardCommandHandler(ILogger<AddWoundedCardCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<WoundedCardDto>> Handle(AddWoundedCardCommand command, CancellationToken ct)
    {
        bool exists = await _context.WoundedCards.AnyAsync(wc => wc.CardNumber == command.CardNumber, ct);
        if (exists)
        {
            _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
            return WoundedCardErrors.CardNumberDuplicated;
        }
        Patient? patient = await _context.Patients.FindAsync(new object[] { command.PatientId }, ct);
        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var woundedCardResult = WoundedCard.Create(command.CardNumber, command.IssueDate, command.ExpirationDate, command.PatientId, command.CardImagePath);
        if (woundedCardResult.IsError)
        {
            return woundedCardResult.Errors;
        }

        var woundedCard = woundedCardResult.Value;
        woundedCard.Patient = patient;

        await _context.WoundedCards.AddAsync(woundedCard, ct);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Wounded card {woundedCardId} is added successfully for patient {patientId}.", woundedCard.Id, patient.Id);

        return woundedCard.ToDto();
    }
}