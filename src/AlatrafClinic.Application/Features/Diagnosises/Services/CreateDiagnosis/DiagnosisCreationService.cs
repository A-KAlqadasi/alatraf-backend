using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Services.Enums;
using AlatrafClinic.Domain.Services.Tickets;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;

public sealed class DiagnosisCreationService : IDiagnosisCreationService
{
    private readonly ILogger<DiagnosisCreationService> _logger;
    private readonly IAppDbContext _context;

    public DiagnosisCreationService(
        ILogger<DiagnosisCreationService> logger,
        IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<Diagnosis>> CreateAsync(
        int ticketId,
        string diagnosisText,
        DateTime injuryDate,
        List<int> injuryReasons,
        List<int> injurySides,
        List<int> injuryTypes,
        DiagnosisType diagnosisType,
        CancellationToken ct)
    {
        var ticket = await _context.Tickets.FirstOrDefaultAsync(t=> t.Id == ticketId, ct);
        if (ticket is null)
        {
            _logger.LogError("Ticket {TicketId} not found.", ticketId);
            return TicketErrors.TicketNotFound;
        }

        if (!ticket.IsEditable)
        {
            _logger.LogError("Ticket {TicketId} is not editable.", ticketId);
            return TicketErrors.ReadOnly;
        }

        if (ticket.Status == TicketStatus.Pause)
        {
            _logger.LogError("Ticket {ticketId} is paused and cannot accept a diagnosis. change it to continue first!", ticketId);
            return TicketErrors.TicketPaused;
        }

        var reasons = new List<InjuryReason>();
        foreach (var id in injuryReasons.Distinct())
        {
            var reason = await _context.InjuryReasons.FirstOrDefaultAsync(i=> i.Id == id, ct);
            if (reason is not null) reasons.Add(reason);
        }

        var sides = new List<InjurySide>();
        foreach (var id in injurySides.Distinct())
        {
            var side = await _context.InjurySides.FirstOrDefaultAsync(i=> i.Id == id, ct);
            if (side is not null) sides.Add(side);
        }

        var types = new List<InjuryType>();
        foreach (var id in injuryTypes.Distinct())
        {
            var type = await _context.InjuryTypes.FirstOrDefaultAsync(i=> i.Id == id, ct);
            if (type is not null) types.Add(type);
        }

        var diagnosisResult = Diagnosis.Create(
            ticketId,
            diagnosisText,
            injuryDate,
            reasons,
            sides,
            types,
            ticket.PatientId!.Value,
            diagnosisType);

        if (diagnosisResult.IsError)
        {
            _logger.LogError("Diagnosis creation failed for ticket {TicketId}: {Errors}", ticketId, diagnosisResult.Errors);
            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;

        _logger.LogInformation("Diagnosis entity instantiated (pending persist) for ticket {TicketId}.", ticketId);
        ticket.Complete();

        return diagnosis;
    }
}