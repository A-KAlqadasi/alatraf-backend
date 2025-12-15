
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly ILogger<GetRoomByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetRoomByIdQueryHandler(ILogger<GetRoomByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery query, CancellationToken ct)
    {
        var room =  await _context.Rooms
        .Include(r => r.Section)
            .ThenInclude(s => s.Department)
        .FirstOrDefaultAsync(r => r.Id == query.RoomId, ct);
        if (room is null)
        {
            _logger.LogError(" Room {RoomId} not found.", query.RoomId);
            return RoomErrors.NotFound;
        }

        return room.ToDto();
    }
}