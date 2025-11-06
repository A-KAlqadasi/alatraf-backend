using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;
public class GetDiagnosesQueryHandler
    : IRequestHandler<GetDiagnosesQuery, Result<PaginatedList<DiagnosisListItemDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetDiagnosesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }
    public async Task<Result<PaginatedList<DiagnosisListItemDto>>> Handle(GetDiagnosesQuery query, CancellationToken ct)
    {
        // var diagnosisQuery = _uow.Diagnoses.AsNoTracking()
        //     .Include(d => d.Patient)!.ThenInclude(p => p.Person) // for PatientName
        //     .Include(d => d.Ticket)                               // for TicketNumber
        //     .Include(d => d.DiagnosisPrograms)!.ThenInclude(dp => dp.MedicalProgram)
        //     .Include(d => d.DiagnosisIndustrialParts)!.ThenInclude(di => di.IndustrialPart)
        //     .Include(d => d.TherapyCards)
        //     .AsQueryable();
        var diagnosisQuery = await _uow.Diagnoses.GetDiagnosesQueryAsync();

        diagnosisQuery = ApplyFilters(diagnosisQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            diagnosisQuery = ApplySearchTerm(diagnosisQuery, query.SearchTerm!);

        diagnosisQuery = ApplySorting(diagnosisQuery, query.SortColumn, query.SortDirection);

        var count = await diagnosisQuery.CountAsync(ct);

        var items = await diagnosisQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(d => new DiagnosisListItemDto
            {
                Id = d.Id,
                DiagnosisText = d.DiagnosisText,
                InjuryDate = d.InjuryDate,
                CreatedAtUtc = d.CreatedAtUtc,

                PatientName = d.Patient != null
                    ? (d.Patient.Person != null
                        ? d.Patient.Person.FullName
                        : d.Patient.ToString()) // fallback if you don’t have Person.FullName
                    : null,

                TicketNumber = d.Ticket != null ? d.Ticket.Id : 0,
                Type = d.DiagnoType,

                Programs = d.DiagnosisPrograms
                    .Select(dp => dp.MedicalProgram != null ? dp.MedicalProgram.Name : string.Empty)
                    .ToList(),

                IndustrialParts = d.DiagnosisIndustrialParts
                    .Select(di =>  di.IndustrialPartUnit != null? ( di.IndustrialPartUnit.IndustrialPart != null ? di.IndustrialPartUnit.IndustrialPart.Name : string.Empty) : string.Empty)
                    .ToList(),

                HasRepairCard = d.RepairCard != null,
                HasSale = d.Sale != null,
                HasTherapyCards = d.TherapyCards.Any()
            })
            .ToListAsync(ct);

        return new PaginatedList<DiagnosisListItemDto>
        {
            Items = items,
            PageNumber = query.Page,
            PageSize = query.PageSize,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)query.PageSize)
        };
    }

    private static IQueryable<Diagnosis> ApplyFilters(IQueryable<Diagnosis> query, GetDiagnosesQuery q)
    {
        if (q.Type.HasValue)
            query = query.Where(d => d.DiagnoType == q.Type.Value);

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
            query = query.Where(d => d.PatientId == q.PatientId.Value);

        if (q.InjuryDateFrom.HasValue)
            query = query.Where(d => d.InjuryDate >= q.InjuryDateFrom.Value);

        if (q.InjuryDateTo.HasValue)
            query = query.Where(d => d.InjuryDate <= q.InjuryDateTo.Value);

        if (q.CreatedDateFrom.HasValue)
            query = query.Where(d => d.CreatedAtUtc >= q.CreatedDateFrom.Value);

        if (q.CreatedDateTo.HasValue)
            query = query.Where(d => d.CreatedAtUtc <= q.CreatedDateTo.Value);

        return query;
    }

    private static IQueryable<Diagnosis> ApplySearchTerm(IQueryable<Diagnosis> query, string searchTerm)
    {
        var normalized = searchTerm.Trim().ToLower();

        return query.Where(d =>
            d.DiagnosisText.ToLower().Contains(normalized) ||
            (d.Patient != null && d.Patient.Person != null &&
                d.Patient.Person.FullName.ToLower().Contains(normalized)) ||
            (d.Ticket != null && d.Ticket.Id.ToString().ToLower().Contains(normalized)) ||
            d.DiagnosisPrograms.Any(dp => dp.MedicalProgram != null && dp.MedicalProgram.Name.ToLower().Contains(normalized)) ||
            d.DiagnosisIndustrialParts.Any(di => di.IndustrialPartUnit != null && di.IndustrialPartUnit.IndustrialPart != null && di.IndustrialPartUnit.IndustrialPart.Name.ToLower().Contains(normalized)) ||
            d.Id.ToString().ToLower().Contains(normalized));
    }

    private static IQueryable<Diagnosis> ApplySorting(IQueryable<Diagnosis> query, string sortColumn, string sortDirection)
    {
        var isDesc = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return sortColumn.ToLower() switch
        {
            "createdat"   => isDesc ? query.OrderByDescending(d => d.CreatedAtUtc) : query.OrderBy(d => d.CreatedAtUtc),
            "injurydate"  => isDesc ? query.OrderByDescending(d => d.InjuryDate)   : query.OrderBy(d => d.InjuryDate),
            "type"        => isDesc ? query.OrderByDescending(d => d.DiagnoType)   : query.OrderBy(d => d.DiagnoType),
            "patient"     => isDesc
                                ? query.OrderByDescending(d => d.Patient!.Person!.FullName)
                                : query.OrderBy(d => d.Patient!.Person!.FullName),
            _             => query.OrderByDescending(d => d.CreatedAtUtc) // default
        };
    }
}