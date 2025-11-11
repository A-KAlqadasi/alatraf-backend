using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;

public sealed record AssignDoctorToRoomCommand(
    int DoctorId,
    int RoomId,
    string? Notes
) : IRequest<Result<Updated>>;
