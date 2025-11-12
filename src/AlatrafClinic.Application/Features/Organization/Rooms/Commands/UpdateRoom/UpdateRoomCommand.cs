using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.UpdateRoom;

public sealed record UpdateRoomCommand(
    int RoomId,
    string NewName
) : IRequest<Result<Updated>>;
