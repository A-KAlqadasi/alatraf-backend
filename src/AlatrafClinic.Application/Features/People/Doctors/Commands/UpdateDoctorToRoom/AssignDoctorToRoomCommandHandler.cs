using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;

public class AssignDoctorToRoomCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<AssignDoctorToRoomCommandHandler> logger
) : IRequestHandler<AssignDoctorToRoomCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<AssignDoctorToRoomCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(AssignDoctorToRoomCommand command, CancellationToken cancellationToken)
  {
    // 1️⃣ Load doctor
    var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.DoctorId, cancellationToken);
    if (doctor is null)
    {
      _logger.LogWarning("Doctor {DoctorId} not found.", command.DoctorId);
      return ApplicationErrors.DoctorNotFound;
    }

    // 2️⃣ Load room
    var room = await _unitOfWork.Rooms.GetByIdAsync(command.RoomId, cancellationToken);
    if (room is null)
    {
      _logger.LogWarning("Room {RoomId} not found.", command.RoomId);
      return ApplicationErrors.RoomNotFound;
    }

    // 3️⃣ Apply domain logic
    var assignResult = doctor.AssignToRoom(room, command.Notes);
    if (assignResult.IsError)
    {
      _logger.LogWarning(
          "Failed to assign Doctor {DoctorId} to new Room {RoomId}: {Error}",
          doctor.Id, room.Id, assignResult.Errors);
      return assignResult.Errors;
    }

    await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation(
        "Doctor {DoctorId} reassigned to new Room {RoomId} (same section).",
        doctor.Id, room.Id);

    return Result.Updated;
  }
}