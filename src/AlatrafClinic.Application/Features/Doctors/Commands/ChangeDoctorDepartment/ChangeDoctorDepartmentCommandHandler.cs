using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.ChangeDoctorDepartment;

public class ChangeDoctorDepartmentCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<ChangeDoctorDepartmentCommandHandler> logger
) : IRequestHandler<ChangeDoctorDepartmentCommand, Result<Updated>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<ChangeDoctorDepartmentCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(ChangeDoctorDepartmentCommand command, CancellationToken cancellationToken)
  {
    var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.DoctorId, cancellationToken);
    if (doctor is null)
    {
      _logger.LogWarning("Doctor {DoctorId} not found.", command.DoctorId);
      return ApplicationErrors.DoctorNotFound;
    }

    // 2️⃣ Fetch new department
    var department = await _unitOfWork.Departments.GetByIdAsync(command.NewDepartmentId, cancellationToken);
    if (department is null)
    {
      _logger.LogWarning("Department {DepartmentId} not found.", command.NewDepartmentId);
      return ApplicationErrors.DepartmentNotFound;
    }

    // 3️⃣ Apply domain logic
    var changeResult = doctor.ChangeDepartment(command.NewDepartmentId);
    if (changeResult.IsError)
    {
      _logger.LogWarning("Failed to change department for Doctor {DoctorId}: {Error}", doctor.Id, changeResult.Errors);
      return changeResult.Errors;
    }

    // 4️⃣ Persist changes
    await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Doctor {DoctorId} transferred to Department {DepartmentId}.", doctor.Id, department.Id);

    return Result.Updated;
  }
}