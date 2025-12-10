using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Application.Features.DisabledCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.AddDisabledCard;

public class AddDisabledCardCommandHandler : IRequestHandler<AddDisabledCardCommand, Result<DisabledCardDto>>
{
    private readonly ILogger<AddDisabledCardCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public AddDisabledCardCommandHandler(ILogger<AddDisabledCardCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<DisabledCardDto>> Handle(AddDisabledCardCommand command, CancellationToken ct)
    {
        bool exists = await _context.DisabledCards.AnyAsync(c=> c.CardNumber == command.CardNumber, ct);

        if (exists)
        {
            _logger.LogWarning("Card number {cardNumber} is already exists!", command.CardNumber);
            return DisabledCardErrors.CardNumberDuplicated;
        }
        Patient? patient = await _context.Patients.FirstOrDefaultAsync(p=> p.Id == command.PatientId, ct);

        if (patient is null)
        {
            _logger.LogError("Patient {id} is not found!", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        var disabledCardResult = DisabledCard.Create(command.CardNumber, command.ExpirationDate, command.PatientId, command.CardImagePath);
        if (disabledCardResult.IsError)
        {
            return disabledCardResult.Errors;
        }

        var disabledCard = disabledCardResult.Value;
        disabledCard.Patient = patient;

        await _context.DisabledCards.AddAsync(disabledCard, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("disabled-card", ct);

        _logger.LogInformation("New disabled card with number {cardNumber} is created successfully", command.CardNumber);

        return disabledCard.ToDto();
    }
}