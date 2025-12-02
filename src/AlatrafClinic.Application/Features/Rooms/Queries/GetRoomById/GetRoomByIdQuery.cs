using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRoomById;

public sealed record GetRoomByIdQuery(
    int RoomId
) : IRequest<Result<RoomDto>>;