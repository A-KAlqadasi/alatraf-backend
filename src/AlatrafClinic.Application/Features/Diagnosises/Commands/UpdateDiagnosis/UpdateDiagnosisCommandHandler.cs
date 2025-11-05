using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Services.Tickets;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.UpdateDiagnosis;

public class UpdateDiagnosisCommandHandler : IRequestHandler<UpdateDiagnosisCommand, Result<Updated>>
{
    private readonly ILogger<UpdateDiagnosisCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _uow;

    public UpdateDiagnosisCommandHandler(ILogger<UpdateDiagnosisCommandHandler> logger, HybridCache cache, IUnitOfWork uow)
    {
        _logger = logger;
        _cache = cache;
        _uow = uow;
    }
    public async Task<Result<Updated>> Handle(UpdateDiagnosisCommand command, CancellationToken ct)
    {
        Diagnosis? diagnosis = await _uow.Diagnosises.GetByIdAsync(command.diagnosisId, ct);
        if (diagnosis is null)
        {
            _logger.LogWarning("Diagnosis with id {DiagnosisId} not found", command.diagnosisId);
            return DiagnosisErrors.DiagnosisNotFound;
        }
        Ticket? ticket = await _uow.Tickets.GetByIdAsync(command.ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with id {TicketId} not found", command.ticketId);
            return TicketErrors.TicketNotFound;
        }
        if(!ticket.IsEditable)
        {
            _logger.LogWarning("Ticket with id {TicketId} is read-only", command.ticketId);
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

        var updateResult = diagnosis.Update(
            command.diagnosisText,
            command.injuryDate,
            injuryReasons,
            injurySides,
            injuryTypes,
            command.diagnosisType);

        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update diagnosis with id {DiagnosisId}: {Error}", command.diagnosisId, updateResult.TopError.Code);
            return updateResult;
        }
        
        await _uow.Diagnosises.UpdateAsync(diagnosis, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Updated;
    }
}