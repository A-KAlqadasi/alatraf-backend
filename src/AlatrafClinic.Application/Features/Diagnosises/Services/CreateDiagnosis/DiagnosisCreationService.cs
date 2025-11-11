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

namespace AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;

public sealed class DiagnosisCreationService : IDiagnosisCreationService
{
    private readonly ILogger<DiagnosisCreationService> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _unitOfWork;

    public DiagnosisCreationService(
        ILogger<DiagnosisCreationService> logger,
        HybridCache cache,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _cache = cache;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Diagnosis>> CreateAsync(
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
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket {TicketId} not found.", ticketId);
            return TicketErrors.TicketNotFound;
        }

        if (!ticket.IsEditable)
        {
            _logger.LogWarning("Ticket {TicketId} is read-only.", ticketId);
            return TicketErrors.ReadOnly;
        }

        // Load injuries by ids (skip nulls gracefully)
        var reasons = new List<InjuryReason>();
        foreach (var id in injuryReasons.Distinct())
        {
            var r = await _unitOfWork.InjuryReasons.GetByIdAsync(id, ct);
            if (r is not null) reasons.Add(r);
        }

        var sides = new List<InjurySide>();
        foreach (var id in injurySides.Distinct())
        {
            var s = await _unitOfWork.InjurySides.GetByIdAsync(id, ct);
            if (s is not null) sides.Add(s);
        }

        var types = new List<InjuryType>();
        foreach (var id in injuryTypes.Distinct())
        {
            var t = await _unitOfWork.InjuryTypes.GetByIdAsync(id, ct);
            if (t is not null) types.Add(t);
        }

        var diagnosisResult = Diagnosis.Create(
            ticketId,
            diagnosisText,
            injuryDate,
            reasons,
            sides,
            types,
            patientId,
            diagnosisType);

        if (diagnosisResult.IsError)
        {
            _logger.LogWarning("Diagnosis creation failed for ticket {TicketId}: {Errors}", ticketId, diagnosisResult.Errors);
            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;
        // Note: handler will add diagnosis to unitOfWork and SaveChanges in a single transaction.
        // We can write-through cache the transient object id after save; for now we just log.
        _logger.LogInformation("Diagnosis entity instantiated (pending persist) for ticket {TicketId}.", ticketId);

        return diagnosis;
    }
}