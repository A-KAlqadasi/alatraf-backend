using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People.Doctors;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.EndDoctorAssignment;

public class EndDoctorAssignmentCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<EndDoctorAssignmentCommandHandler> logger
) : IRequestHandler<EndDoctorAssignmentCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<EndDoctorAssignmentCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(EndDoctorAssignmentCommand command, CancellationToken cancellationToken)
  {
    var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.DoctorId, cancellationToken);
    if (doctor is null)
    {
      _logger.LogWarning("Doctor {DoctorId} not found.", command.DoctorId);
      return ApplicationErrors.DoctorNotFound;
    }

    var activeAssignment = doctor.GetCurrentAssignment();
    if (activeAssignment is null)
    {
      _logger.LogWarning("Doctor {DoctorId} has no active assignment to end.", command.DoctorId);
      return DoctorErrors.NoActiveAssignment;
    }

    var endResult = activeAssignment.EndAssignment();
    if (endResult.IsError)
    {
      _logger.LogWarning(
          "Failed to end assignment for Doctor {DoctorId}: {Error}",
          doctor.Id, endResult.Errors);
      return endResult.Errors;
    }

    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Doctor {DoctorId}'s current assignment ended successfully.", doctor.Id);

    return Result.Updated;
  }
}