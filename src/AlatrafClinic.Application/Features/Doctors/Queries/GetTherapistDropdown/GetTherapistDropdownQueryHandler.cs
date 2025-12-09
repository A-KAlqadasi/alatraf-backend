using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;

public class GetTherapistDropdownQueryHandler : IRequestHandler<GetTherapistDropdownQuery, List<TherapistDto>>
{
    private readonly IAppDbContext _context;

    public GetTherapistDropdownQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<TherapistDto>> Handle(GetTherapistDropdownQuery query, CancellationToken ct)
    {
        var therapistAssignements = await  _context.DoctorSectionRooms
            .Include(dsrm=> dsrm.Doctor).ThenInclude(d=> d.Person)
            .Include(dsrm=> dsrm.Section).ThenInclude(s=> s.Rooms)
            .Where(dsrm => dsrm.IsActive && dsrm.GetTodaySessionsCount() > 0)
            .ToListAsync(ct);

        return therapistAssignements.ToTherapistDtos();
    }
}