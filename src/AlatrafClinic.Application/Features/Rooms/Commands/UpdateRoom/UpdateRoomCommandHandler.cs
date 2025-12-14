using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

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

  public async Task<Result<Updated>> Handle(UpdateRoomCommand request, CancellationToken ct)
  {
    var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId, ct);
    if (room is null)
    {
      _logger.LogError(" Room {RoomId} not found.", request.RoomId);
      return ApplicationErrors.RoomNotFound;
    }

    var updateResult = room.UpdateName(request.NewName);
    if (updateResult.IsError)
    {
      _logger.LogError(" Failed to update Room {RoomId}: {Error}", request.RoomId, updateResult.Errors);
      return updateResult.Errors;
    }

    await _context.SaveChangesAsync(ct);
    await _cache.RemoveByTagAsync("room", ct);
    _logger.LogInformation(" Room {RoomId} number updated successfully to {NewNumber}.",
        room.Id, room.Name);

    return Result.Updated;
  }
}