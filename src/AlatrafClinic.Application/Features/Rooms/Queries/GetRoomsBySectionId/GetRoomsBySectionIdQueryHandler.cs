
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRoomsBySectionId;

public class GetRoomsBySectionIdQueryHandler : IRequestHandler<GetRoomsBySectionIdQuery, Result<List<SectionRoomDto>>>
{
    private readonly IAppDbContext _context;

    public GetRoomsBySectionIdQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<SectionRoomDto>>> Handle(GetRoomsBySectionIdQuery query, CancellationToken ct)
    {
        var rooms = await _context.Rooms
            .Where(r => r.SectionId == query.SectionId)
            .OrderBy(r => r.Name)
            .AsNoTracking()
            .Select(r => new SectionRoomDto
            {
                RoomId = r.Id,
                RoomName = r.Name
            })
            .ToListAsync(ct);

        return rooms;
    }
}