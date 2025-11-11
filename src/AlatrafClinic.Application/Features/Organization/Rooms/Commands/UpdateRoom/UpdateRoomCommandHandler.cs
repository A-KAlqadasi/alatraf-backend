using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.UpdateRoom;

public sealed class UpdateRoomCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<UpdateRoomCommandHandler> logger
) : IRequestHandler<UpdateRoomCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<UpdateRoomCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(UpdateRoomCommand request, CancellationToken ct)
  {
    var room = await _unitOfWork.Rooms.GetRoomByIdWithSectionAsync(request.RoomId, ct);
    if (room is null)
    {
      _logger.LogWarning(" Room {RoomId} not found.", request.RoomId);
      return ApplicationErrors.RoomNotFound;
    }

    var updateResult = room.UpdateNumber(request.NewNumber);
    if (updateResult.IsError)
    {
      _logger.LogWarning(" Failed to update Room {RoomId}: {Error}", request.RoomId, updateResult.Errors);
      return updateResult.Errors;
    }

    await _unitOfWork.Rooms.UpdateAsync(room, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    _logger.LogInformation(" Room {RoomId} number updated successfully to {NewNumber}.",
        room.Id, room.Number);

    return Result.Updated;
  }
}