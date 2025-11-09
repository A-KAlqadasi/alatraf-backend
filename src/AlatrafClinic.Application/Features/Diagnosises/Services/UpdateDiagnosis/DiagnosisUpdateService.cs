using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Services.Tickets;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;

public sealed class DiagnosisUpdateService : IDiagnosisUpdateService
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<DiagnosisUpdateService> _logger;
    private readonly HybridCache _cache;

    public DiagnosisUpdateService(ILogger<DiagnosisUpdateService> logger, HybridCache cache, IUnitOfWork uow)
    {
        _logger = logger;
        _uow = uow;
        _cache = cache;
    }

    public async Task<Result<Diagnosis>> UpdateAsync(
        int diagnosisId,
        int ticketId,
        string diagnosisText,
        DateTime injuryDate,
        List<int> injuryReasons,
        List<int> injurySides,
        List<int> injuryTypes,
        int patientId,
        DiagnosisType diagnosisType,
        CancellationToken ct)
    {
         Diagnosis? diagnosis = await _uow.Diagnoses.GetByIdAsync(diagnosisId, ct);
        if (diagnosis is null)
        {
            _logger.LogWarning("Diagnosis with id {DiagnosisId} not found", diagnosisId);
            return DiagnosisErrors.DiagnosisNotFound;
        }
        Ticket? ticket = await _uow.Tickets.GetByIdAsync(ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with id {TicketId} not found", ticketId);
            return TicketErrors.TicketNotFound;
        }
        
        if (!ticket.IsEditable)
        {
            _logger.LogWarning("Ticket with id {TicketId} is read-only", ticketId);
            return TicketErrors.ReadOnly;
        }
        List<InjuryReason> reasons = new();

        foreach (var reasonId in injuryReasons.Distinct())
        {
            var reason = await _uow.InjuryReasons.GetByIdAsync(reasonId, ct);
            if (reason is not null)
            {
                reasons.Add(reason);
            }
        }

        List<InjuryType> types = new();
        foreach (var typeId in injuryTypes.Distinct())
        {
            var type = await _uow.InjuryTypes.GetByIdAsync(typeId, ct);
            if (type is not null)
            {
                types.Add(type);
            }
        }

        List<InjurySide> sides = new();
        foreach (var sideId in injurySides.Distinct())
        {
            var side = await _uow.InjurySides.GetByIdAsync(sideId, ct);
            if (side is not null)
            {
                sides.Add(side);
            }
        }

        var updateResult = diagnosis.Update(
            diagnosisText,
            injuryDate,
            reasons,
            sides,
            types,
            diagnosisType);

        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update diagnosis with id {DiagnosisId}: {Error}", diagnosisId, updateResult.TopError.Code);
            return updateResult.Errors;
        }

        return diagnosis; 
    }
}