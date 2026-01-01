using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Rooms.Commands.DeleteRoom;

public sealed class DeleteRoomCommandHandler(
    IAppDbContext _context,
    ILogger<DeleteRoomCommandHandler> _logger,
    HybridCache _cache
) : IRequestHandler<DeleteRoomCommand, Result<Deleted>>
{
    public async Task<Result<Deleted>> Handle(DeleteRoomCommand command, CancellationToken ct)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == command.RoomId, ct);
        if (room is null)
        {
            _logger.LogError(" Room {RoomId} not found for deletion.", command.RoomId);
            return ApplicationErrors.RoomNotFound;
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("room", ct);

        _logger.LogInformation("Room {RoomId} deleted successfully.", room.Id);

        return Result.Deleted;
    }
}