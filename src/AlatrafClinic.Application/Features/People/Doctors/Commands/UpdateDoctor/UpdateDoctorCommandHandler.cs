
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Persons.Services.UpdatePerson;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Doctors;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler(
    IPersonUpdateService personUpdateService,
    IUnitOfWork unitOfWork,
    ILogger<UpdateDoctorCommandHandler> logger
) : IRequestHandler<UpdateDoctorCommand, Result<Updated>>
{
  private readonly IPersonUpdateService _personUpdateService = personUpdateService;
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<UpdateDoctorCommandHandler> _logger = logger;

  public async Task<Result<Updated>> Handle(UpdateDoctorCommand command, CancellationToken cancellationToken)
  {
    var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.DoctorId, cancellationToken);
    if (doctor is null)
      return ApplicationErrors.DoctorNotFound;

    var person = await _unitOfWork.Person.GetByIdAsync(doctor.PersonId, cancellationToken);
    if (person is null)
      return ApplicationErrors.PersonNotFound;

    var personUpdate = await _personUpdateService.UpdateAsync(
        person.Id,
        command.Fullname,
        command.Birthdate,
        command.Phone,
        command.NationalNo,
        command.Address,
        cancellationToken);

    if (personUpdate.IsError)
      return personUpdate.Errors;

    var specUpdate = doctor.UpdateSpecialization(command.Specialization);
    if (specUpdate.IsError)
      return specUpdate.Errors;



    await _unitOfWork.Person.UpdateAsync(person, cancellationToken);
    await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    _logger.LogInformation("Doctor {DoctorId} and Person {PersonId} updated successfully.", doctor.Id, person.Id);
    return Result.Updated;
  }
}
