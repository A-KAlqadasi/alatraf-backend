using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Rooms.Commands.UpdateRoom;

public sealed record UpdateRoomCommand(
    int RoomId,
    string NewName
) : IRequest<Result<Updated>>;
