

using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.DeleteRoom;

public sealed record DeleteRoomCommand(
    int RoomId
) : IRequest<Result<Deleted>>;
