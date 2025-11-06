using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.CreateDiagnosis;

public class CreateDiagnosisCommandHandler: IRequestHandler<CreateDiagnosisCommand, Result<DiagnosisDto>>
{
    private readonly ILogger<CreateDiagnosisCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _uow;

    public CreateDiagnosisCommandHandler(ILogger<CreateDiagnosisCommandHandler> logger, HybridCache cache, IUnitOfWork uow)
    {
        _logger = logger;
        _cache = cache;
        _uow = uow;
    }
    public async Task<Result<DiagnosisDto>> Handle(CreateDiagnosisCommand command, CancellationToken ct)
    {
        Ticket? ticket = await _uow.Tickets.GetByIdAsync(command.ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with id {TicketId} not found", command.ticketId);
            return TicketErrors.TicketNotFound;
        }
        if (!ticket.IsEditable)
        {
            _logger.LogWarning("Ticket with id {TicketId} is not editable", command.ticketId);
            return TicketErrors.ReadOnly;
        }

        List<InjuryReason> injuryReasons = new();
        foreach (var reasonId in command.injuryReasons)
        {
            var reason = await _uow.InjuryReasons.GetByIdAsync(reasonId, ct);
            if (reason is not null)
            {
                injuryReasons.Add(reason);
            }
        }

        List<InjuryType> injuryTypes = new();
        foreach (var typeId in command.injuryTypes)
        {
            var type = await _uow.InjuryTypes.GetByIdAsync(typeId, ct);
            if (type is not null)
            {
                injuryTypes.Add(type);
            }
        }

        List<InjurySide> injurySides = new();
        foreach (var sideId in command.injurySides)
        {
            var side = await _uow.InjurySides.GetByIdAsync(sideId, ct);
            if (side is not null)
            {
                injurySides.Add(side);
            }
        }
        var diagnosisResult = Diagnosis.Create(
            command.ticketId,
            command.diagnosisText,
            command.injuryDate,
            injuryReasons,
            injurySides,
            injuryTypes,
            command.patientId,
            command.diagnosisType
        );

        if (diagnosisResult.IsError)
        {
            _logger.LogWarning("Failed to create diagnosis for ticket {TicketId}: {Error}", command.ticketId, diagnosisResult.Errors);
            return diagnosisResult.Errors;
        }

        await _uow.Diagnoses.AddAsync(diagnosisResult.Value, ct);
        await _uow.SaveChangesAsync(ct);
        
        _logger.LogInformation("Diagnosis created successfully with Id {DiagnosisId} for ticket {TicketId}", diagnosisResult.Value.Id, command.ticketId);

        return diagnosisResult.Value.ToDto();
    }
}