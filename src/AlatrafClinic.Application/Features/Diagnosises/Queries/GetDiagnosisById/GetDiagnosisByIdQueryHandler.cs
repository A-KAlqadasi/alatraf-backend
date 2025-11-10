using Microsoft.Extensions.Logging;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnosisById;

public class GetDiagnosisByIdQueryHandler
    : IRequestHandler<GetDiagnosisByIdQuery, Result<DiagnosisDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<GetDiagnosisByIdQueryHandler> _logger;

    public GetDiagnosisByIdQueryHandler(IUnitOfWork uow, ILogger<GetDiagnosisByIdQueryHandler> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    public async Task<Result<DiagnosisDto>> Handle(GetDiagnosisByIdQuery query, CancellationToken ct)
    {
        var diagnosis = await _uow.Diagnoses.GetByIdAsync(query.DiagnosisId, ct);

        // var diagnosis = await baseQuery
        //     .Include(d => d.Patient)!.ThenInclude(p => p.Person)
        //     .Include(d => d.Ticket)
        //     .Include(d => d.InjuryReasons)
        //     .Include(d => d.InjurySides)
        //     .Include(d => d.InjuryTypes)
        //     .Include(d => d.DiagnosisPrograms)!.ThenInclude(dp => dp.MedicalProgram)
        //     .Include(d => d.DiagnosisIndustrialParts)!
        //         .ThenInclude(di => di.IndustrialPartUnit)!.ThenInclude(ipu => ipu.Unit)
        //     .Include(d => d.DiagnosisIndustrialParts)!
        //         .ThenInclude(di => di.IndustrialPartUnit)!.ThenInclude(ipu => ipu.IndustrialPart)
        //     .Include(d => d.Sale)!.ThenInclude(s => s.SaleItems)!.ThenInclude(si => si.ItemUnit)!.ThenInclude(iu => iu.Item)
        //     .Include(d => d.Sale)!.ThenInclude(s => s.SaleItems)!.ThenInclude(si => si.ItemUnit)!.ThenInclude(iu => iu.Unit)
        //     .Include(d => d.TherapyCards)
        //     .Include(d => d.RepairCard)
        //     .FirstOrDefaultAsync(d => d.Id == query.DiagnosisId, ct);

        if (diagnosis is null)
        {
            _logger.LogWarning("Diagnosis not found: {DiagnosisId}", query.DiagnosisId);
            return DiagnosisErrors.DiagnosisNotFound;
        }

        return diagnosis.ToDto();
    }
}