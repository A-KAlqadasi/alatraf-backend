
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Doctors.Dtos;
using AlatrafClinic.Application.Features.People.Doctors.Mappers;
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Doctors;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler(
      IPersonCreateService personCreateService,
      IUnitOfWork unitOfWork,
      ILogger<CreateDoctorCommandHandler> logger
  ) : IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>
{
  private readonly IPersonCreateService _personCreateService = personCreateService;
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<CreateDoctorCommandHandler> _logger = logger;

  public async Task<Result<DoctorDto>> Handle(CreateDoctorCommand command, CancellationToken cancellationToken)
  {
    var personResult = await _personCreateService.CreateAsync(command.Person,
        cancellationToken);

    if (personResult.IsError)
      return personResult.Errors;

    var person = personResult.Value;



    var department = await _unitOfWork.Departments.GetByIdAsync(command.DepartmentId, cancellationToken);
    if (department is null)
    {
      _logger.LogWarning("Department with ID {DepartmentId} not found.", command.DepartmentId);
      return ApplicationErrors.DepartmentNotFound;
    }

    var doctorResult = Doctor.Create(
        person.Id,
        command.DepartmentId,
        command.Specialization);

    if (doctorResult.IsError)
      return doctorResult.Errors;

    var doctor = doctorResult.Value;

    await _unitOfWork.People.AddAsync(person, cancellationToken);
    await _unitOfWork.Doctors.AddAsync(doctor, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Doctor created successfully with ID: {DoctorId}", doctor.Id);

    return doctor.ToDto();
  }
}