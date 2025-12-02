using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToSectionAndRoom;

public class AssignDoctorToSectionAndRoomCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<AssignDoctorToSectionAndRoomCommandHandler> logger
) : IRequestHandler<AssignDoctorToSectionAndRoomCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<AssignDoctorToSectionAndRoomCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(AssignDoctorToSectionAndRoomCommand command, CancellationToken cancellationToken)
  {
    var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.DoctorId, cancellationToken);
    if (doctor is null)
    {
      _logger.LogWarning("Doctor {DoctorId} not found.", command.DoctorId);
      return ApplicationErrors.DoctorNotFound;
    }

    var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, cancellationToken);
    if (section is null)
    {
      _logger.LogWarning("Section {SectionId} not found.", command.SectionId);
      return ApplicationErrors.SectionNotFound;
    }

    var room = await _unitOfWork.Rooms.GetByIdAsync(command.RoomId, cancellationToken);
    if (room is null)
    {
      _logger.LogWarning("Room {RoomId} not found.", command.RoomId);
      return ApplicationErrors.RoomNotFound;
    }

    var assignResult = doctor.AssignToSectionAndRoom(section, room, command.Notes);
    if (assignResult.IsError)
    {
      _logger.LogWarning("Failed to assign Doctor {DoctorId} to Section {SectionId} / Room {RoomId}: {Error}",
          doctor.Id, section.Id, room.Id, assignResult.Errors);
      return assignResult.Errors;
    }

    await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Doctor {DoctorId} assigned to Section {SectionId} / Room {RoomId}.",
        doctor.Id, section.Id, room.Id);

    return Result.Updated;
  }
}