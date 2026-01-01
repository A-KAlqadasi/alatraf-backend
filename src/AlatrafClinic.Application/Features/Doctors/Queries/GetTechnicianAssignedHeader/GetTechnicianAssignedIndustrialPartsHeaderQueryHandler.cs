
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedHeader;

public sealed class GetTechnicianAssignedHeaderQueryHandler
    : IRequestHandler<GetTechnicianAssignedHeaderQuery, Result<TechnicianHeaderDto>>
{
    private readonly IAppDbContext _context;

    public GetTechnicianAssignedHeaderQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TechnicianHeaderDto>> Handle(
        GetTechnicianAssignedHeaderQuery query,
        CancellationToken ct)
    {

        var header = await _context.DoctorSectionRooms
            .AsNoTracking()
            .Where(a => a.Id == query.DoctorSectionRoomId && a.IsActive)
            .Select(a => new TechnicianHeaderDto
            {
                DoctorSectionRoomId = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.Person!.FullName,
                SectionId = a.SectionId,
                SectionName = a.Section.Name,
            })
            .FirstOrDefaultAsync(ct);

        if (header is null)
        {
            return Error.NotFound(
                "DoctorSectionRoom.NotFound",
                "Doctor section room not found or inactive.");
        }

        return header;
    }
}