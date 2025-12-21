
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedHeader;

public sealed class GetTechnicianAssignedIndustrialPartsHeaderQueryHandler
    : IRequestHandler<GetTechnicianAssignedIndustrialPartsHeaderQuery, Result<TechnicianAssignedIndustrialPartsHeaderDto>>
{
    private readonly IAppDbContext _context;

    public GetTechnicianAssignedIndustrialPartsHeaderQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<TechnicianAssignedIndustrialPartsHeaderDto>> Handle(
        GetTechnicianAssignedIndustrialPartsHeaderQuery query,
        CancellationToken ct)
    {
        var today = AlatrafClinicConstants.TodayDate;

        var header = await _context.DoctorSectionRooms
            .AsNoTracking()
            .Where(a => a.Id == query.DoctorSectionRoomId && a.IsActive)
            .Select(a => new TechnicianAssignedIndustrialPartsHeaderDto
            {
                DoctorSectionRoomId = a.Id,
                DoctorId = a.DoctorId,
                DoctorName = a.Doctor.Person!.FullName,
                SectionId = a.SectionId,
                SectionName = a.Section.Name,
                Date = today
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