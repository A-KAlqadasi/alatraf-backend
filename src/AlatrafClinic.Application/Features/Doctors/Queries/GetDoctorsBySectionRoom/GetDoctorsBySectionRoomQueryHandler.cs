using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctorsBySectionRoom;

public sealed class GetDoctorsBySectionRoomQueryHandler
    : IRequestHandler<GetDoctorsBySectionRoomQuery, Result<List<GetDoctorDto>>>
{
    private readonly IAppDbContext _db;

    public GetDoctorsBySectionRoomQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<List<GetDoctorDto>>> Handle(
        GetDoctorsBySectionRoomQuery query,
        CancellationToken ct)
    {
        var assignments = _db.DoctorSectionRooms
            .Include(d=> d.Doctor)
            .ThenInclude(d=> d.Person)
            .AsNoTracking()
            .Where(a => a.IsActive && a.SectionId == query.SectionId);

        if (query.RoomId.HasValue && query.RoomId.Value > 0)
        {
            assignments = assignments.Where(a => a.RoomId == query.RoomId.Value);
        }

        var doctors = await assignments
            .Select(a => a.Doctor)
            .Distinct()
            .OrderBy(d => d.Person!.FullName)
            .Select(d => new GetDoctorDto
            {
                Id = d.Id,
                Name = d.Person!.FullName,
            })
            .ToListAsync(ct);

        return doctors;
    }

}