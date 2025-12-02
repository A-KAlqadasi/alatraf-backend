using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Rooms.Commands.DeleteRoom;

public sealed class DeleteRoomCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<DeleteRoomCommandHandler> logger,
    ICacheService cacheService
) : IRequestHandler<DeleteRoomCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<DeleteRoomCommandHandler> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<Deleted>> Handle(DeleteRoomCommand request, CancellationToken ct)
    {
        var room = await _unitOfWork.Rooms.GetByIdAsync(request.RoomId, ct);
        if (room is null)
        {
            _logger.LogError(" Room {RoomId} not found for deletion.", request.RoomId);
            return ApplicationErrors.RoomNotFound;
        }

        await _unitOfWork.Rooms.DeleteAsync(room, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Room {RoomId} deleted successfully.", room.Id);

        await _cacheService.RemoveByTagAsync("room", ct);

        return Result.Deleted;
    }
}