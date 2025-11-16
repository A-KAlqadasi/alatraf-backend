using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.DeleteRoom;

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

        var hasActiveAssignment = await _unitOfWork.DoctorSectionRooms.HasActiveAssignmentByRoomIdAsync(request.RoomId, ct);
        if (hasActiveAssignment)
        {
            _logger.LogError("Room {RoomId} cannot be deleted because it has active doctor assignments.", request.RoomId);
            return ApplicationErrors.RoomHasActiveDoctorAssignment;
        }

        var deleteResult = room.SoftDelete();
        if (deleteResult.IsError)
            return deleteResult.Errors;

        await _unitOfWork.Rooms.UpdateAsync(room, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(" Room {RoomId} soft-deleted successfully.", room.Id);

        await _cacheService.RemoveByTagAsync("room", ct);

        return Result.Deleted;
    }
}