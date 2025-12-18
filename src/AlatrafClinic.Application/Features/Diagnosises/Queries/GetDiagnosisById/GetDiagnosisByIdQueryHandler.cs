using Microsoft.Extensions.Logging;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnosisById;

public class GetDiagnosisByIdQueryHandler
    : IRequestHandler<GetDiagnosisByIdQuery, Result<DiagnosisDto>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetDiagnosisByIdQueryHandler> _logger;

    public GetDiagnosisByIdQueryHandler(IAppDbContext context, ILogger<GetDiagnosisByIdQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<DiagnosisDto>> Handle(GetDiagnosisByIdQuery query, CancellationToken ct)
    {
        var diagnosis =  await _context.Diagnoses
            .Include(d => d.Patient)!.ThenInclude(p => p.Person)
            .Include(d => d.Ticket)
            .Include(d => d.InjuryReasons)
            .Include(d => d.InjurySides)
            .Include(d => d.InjuryTypes)
            .Include(d => d.DiagnosisPrograms)!.ThenInclude(dp => dp.MedicalProgram)
            .Include(d => d.DiagnosisIndustrialParts)!
                .ThenInclude(di => di.IndustrialPartUnit)!.ThenInclude(ipu => ipu.Unit)
            .Include(d => d.DiagnosisIndustrialParts)!
                .ThenInclude(di => di.IndustrialPartUnit)!.ThenInclude(ipu => ipu.IndustrialPart)
            .Include(d => d.Sale!).ThenInclude(s => s.SaleItems).ThenInclude(si => si.ItemUnit)!.ThenInclude(iu => iu.Item)
            .Include(d => d.Sale!).ThenInclude(s => s.SaleItems)!.ThenInclude(si => si.ItemUnit)!.ThenInclude(iu => iu.Unit)
            .Include(d => d.TherapyCard)
            .Include(d => d.RepairCard)
            .FirstOrDefaultAsync(d => d.Id == query.DiagnosisId, ct);

        if (diagnosis is null)
        {
            _logger.LogWarning("Diagnosis not found: {DiagnosisId}", query.DiagnosisId);
            return DiagnosisErrors.DiagnosisNotFound;
        }

        return diagnosis.ToDto();
    }
}