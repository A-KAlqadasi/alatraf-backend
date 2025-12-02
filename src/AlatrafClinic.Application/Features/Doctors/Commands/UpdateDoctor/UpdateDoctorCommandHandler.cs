using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Services.UpdatePerson;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler(
    IPersonUpdateService personUpdateService,
    IUnitOfWork unitOfWork,
    ILogger<UpdateDoctorCommandHandler> logger
) : IRequestHandler<UpdateDoctorCommand, Result<Updated>>
{
    private readonly IPersonUpdateService _personUpdateService = personUpdateService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<UpdateDoctorCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(UpdateDoctorCommand command, CancellationToken ct)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.DoctorId, ct);
        if (doctor is null)
        return ApplicationErrors.DoctorNotFound;

        var person = await _unitOfWork.People.GetByIdAsync(doctor.PersonId, ct);
        if (person is null)
        return ApplicationErrors.PersonNotFound;

        var personUpdate = await _personUpdateService.UpdateAsync(
            person.Id,
            command.Fullname,
            command.Birthdate,
            command.Phone,
            command.NationalNo,
            command.Address,
            command.Gender,
            ct);

        if (personUpdate.IsError)
        return personUpdate.Errors;

        var specUpdate = doctor.UpdateSpecialization(command.Specialization);
        if (specUpdate.IsError)
        return specUpdate.Errors;



        await _unitOfWork.People.UpdateAsync(person, ct);
        await _unitOfWork.Doctors.UpdateAsync(doctor, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Doctor {DoctorId} and Person {PersonId} updated successfully.", doctor.Id, person.Id);
        return Result.Updated;
    }
}
