using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Rooms.Commands.UpdateRoom;

public sealed class UpdateRoomCommandHandler(
    IAppDbContext _context,
    ILogger<UpdateRoomCommandHandler> _logger,
    HybridCache _cache
) : IRequestHandler<UpdateRoomCommand, Result<Updated>>
{

  public async Task<Result<Updated>> Handle(UpdateRoomCommand command, CancellationToken ct)
  {
    var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == command.RoomId, ct);
    if (room is null)
    {
      _logger.LogError(" Room {RoomId} not found.", command.RoomId);
      return ApplicationErrors.RoomNotFound;
    }

     var isRoomExists = await _context.Sections
            .Where(s => s.Id == room.SectionId)
            .SelectMany(s => s.Rooms)
            .AnyAsync(r => r.Name == command.NewName, ct);
        if (isRoomExists && room.Name != command.NewName)
        {
            _logger.LogError(" Room with name {RoomName} already exists in Section {SectionId}.",
                command.NewName, room.SectionId);
            return RoomErrors.DuplicateRoomName;
        }

    var updateResult = room.UpdateName(command.NewName);
    if (updateResult.IsError)
    {
        _logger.LogError(" Failed to update Room {RoomId}: {Error}", command.RoomId, updateResult.Errors);
        return updateResult.Errors;
    }

    await _context.SaveChangesAsync(ct);
    await _cache.RemoveByTagAsync("room", ct);
    _logger.LogInformation(" Room {RoomId} number updated successfully to {NewNumber}.",
        room.Id, room.Name);

    return Result.Updated;
  }
}