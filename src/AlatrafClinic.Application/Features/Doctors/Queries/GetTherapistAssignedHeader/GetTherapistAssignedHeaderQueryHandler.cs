using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistAssignedHeader;

public sealed class GetTherapistAssignedHeaderQueryHandler
    : IRequestHandler<GetTherapistAssignedHeaderQuery, Result<TherapistHeaderDto>>
{
    private readonly IAppDbContext _context;

    public GetTherapistAssignedHeaderQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TherapistHeaderDto>> Handle(
        GetTherapistAssignedHeaderQuery query,
        CancellationToken ct)
    {
        var header = await _context.DoctorSectionRooms
            .AsNoTracking()
            .Where(a => a.Id == query.DoctorSectionRoomId && a.IsActive)
            .Select(a => new TherapistHeaderDto
            {
                DoctorSectionRoomId = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.Person!.FullName,
                SectionId = a.SectionId,
                SectionName = a.Section.Name,
                RoomId = a.RoomId,
                RoomName = a.Room!.Name,
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