using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Services.Tickets;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;

public sealed class DiagnosisUpdateService : IDiagnosisUpdateService
{
    private readonly ILogger<DiagnosisUpdateService> _logger;
    private readonly IAppDbContext _context;

    public DiagnosisUpdateService(ILogger<DiagnosisUpdateService> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<Diagnosis>> UpdateAsync(
        int diagnosisId,
        int ticketId,
        string diagnosisText,
        DateOnly injuryDate,
        List<int> injuryReasons,
        List<int> injurySides,
        List<int> injuryTypes,
        DiagnosisType diagnosisType,
        CancellationToken ct)
    {
        Diagnosis? diagnosis = await _context.Diagnoses
        .Include(d=> d.InjuryReasons)
        .Include(d=> d.InjurySides)
        .Include(d=> d.InjuryTypes)
        .Include(d=> d.DiagnosisIndustrialParts)
        .Include(d=> d.DiagnosisPrograms)
        .FirstOrDefaultAsync(d=> d.Id == diagnosisId, ct);

        if (diagnosis is null)
        {
            _logger.LogError("Diagnosis with id {DiagnosisId} not found", diagnosisId);

            return DiagnosisErrors.DiagnosisNotFound;
        }
        Ticket? ticket = await _context.Tickets.FirstOrDefaultAsync(t=> t.Id == ticketId, ct);
        
        if (ticket is null)
        {
            _logger.LogError("Ticket with id {TicketId} not found", ticketId);

            return TicketErrors.TicketNotFound;
        }
        
        
        List<InjuryReason> reasons = new();

        foreach (var reasonId in injuryReasons.Distinct())
        {
            var reason = await _context.InjuryReasons.FirstOrDefaultAsync(i => i.Id == reasonId, ct);
            if (reason is not null)
            {
                reasons.Add(reason);
            }
        }

        List<InjuryType> types = new();
        foreach (var typeId in injuryTypes.Distinct())
        {
            var type = await _context.InjuryTypes.FirstOrDefaultAsync(i=> i.Id == typeId, ct);
            if (type is not null)
            {
                types.Add(type);
            }
        }

        List<InjurySide> sides = new();
        foreach (var sideId in injurySides.Distinct())
        {
            var side = await _context.InjurySides.FirstOrDefaultAsync(i=> i.Id == sideId, ct);
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
            _logger.LogError("Failed to update diagnosis with id {DiagnosisId}: {Error}", diagnosisId, updateResult.TopError);
            return updateResult.Errors;
        }

        return diagnosis; 
    }
}