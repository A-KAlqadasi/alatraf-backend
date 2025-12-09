using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;

public class GetTechniciansDropdownQueryHandler : IRequestHandler<GetTechniciansDropdownQuery, List<TechnicianDto>>
{
    private readonly IAppDbContext _context;

    public GetTechniciansDropdownQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<TechnicianDto>> Handle(GetTechniciansDropdownQuery query, CancellationToken ct)
    {
        var technicianAssignements =await _context.DoctorSectionRooms
            .Include(dsrm=> dsrm.DiagnosisIndustrialParts)
            .Include(dsrm=> dsrm.Doctor).ThenInclude(d=> d.Person)
            .Include(dsrm=> dsrm.Section)
            .Where(dsrm => dsrm.IsActive && dsrm.GetTodayIndustrialPartsCount() > 0)
            .ToListAsync(ct);

        return technicianAssignements.ToTechnicianDtos();
    }
}