using Microsoft.EntityFrameworkCore;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;

using MediatR;

namespace AlatrafClinic.Application.Features.Diagnosises.Queries.GetDiagnoses;

public class GetDiagnosesQueryHandler
    : IRequestHandler<GetDiagnosesQuery, Result<PaginatedList<DiagnosisDto>>>
{
    private readonly IUnitOfWork _uow;

    public GetDiagnosesQueryHandler(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<PaginatedList<DiagnosisDto>>> Handle(GetDiagnosesQuery query, CancellationToken ct)
    {
        // Base query – no Include (server-side projection handles joins)
        var diagnosisQuery = await _uow.Diagnoses.GetDiagnosesQueryAsync();

        diagnosisQuery = ApplyFilters(diagnosisQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            diagnosisQuery = ApplySearchTerm(diagnosisQuery, query.SearchTerm!);

        diagnosisQuery = ApplySorting(diagnosisQuery, query.SortColumn, query.SortDirection);

        // Paging guard
        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;

        var count = await diagnosisQuery.CountAsync(ct);

        // Project directly into DiagnosisDto (lightweight; collections left null for list)
        var items = await diagnosisQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(d => new DiagnosisDto
            {
                DiagnosisId   = d.Id,
                DiagnosisText = d.DiagnosisText,
                InjuryDate    = d.InjuryDate,

                TicketId      = d.TicketId,
                PatientId     = d.PatientId,
                PatientName   = d.Patient != null && d.Patient.Person != null
                                    ? d.Patient.Person.FullName
                                    : string.Empty,

                DiagnosisType = d.DiagnoType,

                InjuryReasons = d.InjuryReasons.ToDtos(),
                InjurySides   = d.InjurySides.ToDtos(),
                InjuryTypes   = d.InjuryTypes.ToDtos(),

                // flags (helpful in list)
                HasRepairCard   = d.RepairCard != null,
                HasSale         = d.Sale != null,
                HasTherapyCards = d.TherapyCards.Any(),

                // keep heavy related collections null for list (Programs/IndustrialParts/SaleItems)
                // Patient object also omitted here to keep list slim
            })
            .ToListAsync(ct);

        return new PaginatedList<DiagnosisDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
    }

    // -------------------- FILTERS --------------------
    private static IQueryable<Diagnosis> ApplyFilters(IQueryable<Diagnosis> query, GetDiagnosesQuery q)
    {
        if (q.Type.HasValue)
            query = query.Where(d => d.DiagnoType == q.Type.Value);

        if (q.PatientId.HasValue && q.PatientId.Value > 0)
            query = query.Where(d => d.PatientId == q.PatientId.Value);

        if (q.TicketId.HasValue && q.TicketId.Value > 0)
            query = query.Where(d => d.TicketId == q.TicketId.Value);

        if (q.HasRepairCard.HasValue)
            query = q.HasRepairCard.Value
                ? query.Where(d => d.RepairCard != null)
                : query.Where(d => d.RepairCard == null);

        if (q.HasSale.HasValue)
            query = q.HasSale.Value
                ? query.Where(d => d.Sale != null)
                : query.Where(d => d.Sale == null);

        if (q.HasTherapyCards.HasValue)
            query = q.HasTherapyCards.Value
                ? query.Where(d => d.TherapyCards.Any())
                : query.Where(d => !d.TherapyCards.Any());

        if (q.InjuryDateFrom.HasValue)
            query = query.Where(d => d.InjuryDate >= q.InjuryDateFrom.Value);

        if (q.InjuryDateTo.HasValue)
            query = query.Where(d => d.InjuryDate <= q.InjuryDateTo.Value);

        if (q.CreatedDateFrom.HasValue)
            query = query.Where(d => d.CreatedAtUtc >= DateTime.SpecifyKind(q.CreatedDateFrom.Value, DateTimeKind.Utc));

        if (q.CreatedDateTo.HasValue)
            query = query.Where(d => d.CreatedAtUtc <= DateTime.SpecifyKind(q.CreatedDateTo.Value, DateTimeKind.Utc));

        return query;
    }

    // -------------------- SEARCH --------------------
    private static IQueryable<Diagnosis> ApplySearchTerm(IQueryable<Diagnosis> query, string searchTerm)
    {
        var pattern = $"%{searchTerm.Trim().ToLower()}%";

        return query.Where(d =>
            EF.Functions.Like(d.DiagnosisText.ToLower(), pattern) ||
            EF.Functions.Like(d.Id.ToString(), pattern) ||
            (d.Patient != null && d.Patient.Person != null &&
                EF.Functions.Like(d.Patient.Person.FullName.ToLower(), pattern)) ||
            (d.Ticket != null && EF.Functions.Like(d.Ticket.Id.ToString(), pattern)) ||
            d.DiagnosisPrograms.Any(dp => dp.MedicalProgram != null &&
                EF.Functions.Like(dp.MedicalProgram.Name.ToLower(), pattern)) ||
            d.DiagnosisIndustrialParts.Any(di =>
                di.IndustrialPartUnit != null &&
                di.IndustrialPartUnit.IndustrialPart != null &&
                EF.Functions.Like(di.IndustrialPartUnit.IndustrialPart.Name.ToLower(), pattern))
        );
    }

    // -------------------- SORTING --------------------
    private static IQueryable<Diagnosis> ApplySorting(IQueryable<Diagnosis> query, string sortColumn, string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "createdat";
        var isDesc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "createdat"  => isDesc ? query.OrderByDescending(d => d.CreatedAtUtc) : query.OrderBy(d => d.CreatedAtUtc),
            "injurydate" => isDesc ? query.OrderByDescending(d => d.InjuryDate) : query.OrderBy(d => d.InjuryDate),
            "type"       => isDesc ? query.OrderByDescending(d => d.DiagnoType) : query.OrderBy(d => d.DiagnoType),
            "patient"    => isDesc
                ? query.OrderByDescending(d => d.Patient != null && d.Patient.Person != null
                    ? d.Patient.Person.FullName
                    : string.Empty)
                : query.OrderBy(d => d.Patient != null && d.Patient.Person != null
                    ? d.Patient.Person.FullName
                    : string.Empty),
            "ticket"     => isDesc
                ? query.OrderByDescending(d => d.Ticket != null ? d.Ticket.Id : 0)
                : query.OrderBy(d => d.Ticket != null ? d.Ticket.Id : 0),
            _            => query.OrderByDescending(d => d.CreatedAtUtc)
        };
    }
}