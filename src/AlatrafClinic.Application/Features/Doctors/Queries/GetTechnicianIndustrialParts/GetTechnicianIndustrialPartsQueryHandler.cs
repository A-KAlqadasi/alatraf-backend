using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianIndustrialParts;

public class GetTechnicianIndustrialPartsQueryHandler : IRequestHandler<GetTechnicianIndustrialPartsQuery, Result<PaginatedList<TechnicianIndustrialPartDto>>>
{
    private readonly IAppDbContext _context;

    public GetTechnicianIndustrialPartsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<PaginatedList<TechnicianIndustrialPartDto>>> Handle(GetTechnicianIndustrialPartsQuery query, CancellationToken ct)
    {
        var filterDate = query.date ?? AlatrafClinicConstants.TodayDate;

        IQueryable<TechnicianIndustrialPartDto> partsQuery = _context.DiagnosisIndustrialParts
            .AsNoTracking()
            .Where(dip =>
                dip.DoctorSectionRoomId == query.DoctorSectionRoomId &&
                dip.DoctorAssignDate == filterDate)
            .Select(dip => new TechnicianIndustrialPartDto
            {
                DiagnosisIndustrialPartId = dip.Id,
                IndustrialPartUnitId = dip.IndustrialPartUnitId,
                Quantity = dip.Quantity,
                IndustrialPartName = dip.IndustrialPartUnit.IndustrialPart.Name,
                UnitName = dip.IndustrialPartUnit.Unit!.Name,
                RepairCardId = dip.RepairCardId,
                PatientName = dip.RepairCard!.Diagnosis.Patient.Person.FullName,
                PatientPhoneNumber = dip.RepairCard.Diagnosis.Patient.Person.Phone
            });

        // 2. Apply optional filters on the DTO query
        partsQuery = ApplyFilters(partsQuery, query);
        
        // 3. Manually handle pagination
        var totalCount = await partsQuery.CountAsync(ct);

        var page = query.Page < 1 ? 1 : query.Page;
        var pageSize = query.PageSize < 1 ? 10 : query.PageSize;
        var skip = (page - 1) * pageSize;

        var items = await partsQuery
            .OrderByDescending(p => p.DiagnosisIndustrialPartId)
            .Skip(skip)
            .Take(pageSize)
            .ToListAsync(ct);

        // 4. Construct the PaginatedList object
        var pagedResult = new PaginatedList<TechnicianIndustrialPartDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };

        return pagedResult;
    }

    // Filter method now operates on IQueryable<TechnicianIndustrialPartDto>
    private static IQueryable<TechnicianIndustrialPartDto> ApplyFilters(
        IQueryable<TechnicianIndustrialPartDto> query,
        GetTechnicianIndustrialPartsQuery q)
    {
        // Filter by RepairCard ID
        if (q.repairCardId.HasValue)
        {
            query = query.Where(x => x.RepairCardId == q.repairCardId.Value);
        }

        if (!string.IsNullOrWhiteSpace(q.patientName))
        {
            var searchTerm = q.patientName.Trim();
            query = query.Where(x => x.PatientName!.Contains(searchTerm));
        }

        return query;
    }
}
