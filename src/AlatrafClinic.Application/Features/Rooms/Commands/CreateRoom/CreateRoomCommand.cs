using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Rooms.Commands.CreateRoom;

public sealed record CreateRoomCommand(
    int SectionId,
    string Name
) : IRequest<Result<RoomDto>>;
