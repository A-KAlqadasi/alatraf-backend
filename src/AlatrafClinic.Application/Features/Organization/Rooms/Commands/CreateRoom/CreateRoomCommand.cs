using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.Organization.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.CreateRoom;

public sealed record CreateRoomCommand(
    int SectionId,
    List<string> RoomNames
) : IRequest<Result<List<RoomDto>>>;
