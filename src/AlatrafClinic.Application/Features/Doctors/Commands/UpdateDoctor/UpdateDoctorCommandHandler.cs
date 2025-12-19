using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Services.UpdatePerson;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.UpdateDoctor;

public class UpdateDoctorCommandHandler(
    IPersonUpdateService _personUpdateService,
    IAppDbContext _context,
    ILogger<UpdateDoctorCommandHandler> _logger
) : IRequestHandler<UpdateDoctorCommand, Result<Updated>>
{
   

    public async Task<Result<Updated>> Handle(UpdateDoctorCommand command, CancellationToken ct)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d=> d.Id == command.DoctorId, ct);
        if (doctor is null)
        {
            _logger.LogError("Doctor with Id {doctorId} is not found", command.DoctorId);
            return ApplicationErrors.DoctorNotFound;
        }

        var person = await _context.People.FirstOrDefaultAsync(p=> p.Id == doctor.PersonId, ct);
        if (person is null)
        {
            _logger.LogError("Person with Id {personId} is not found, for updating doctor", doctor.PersonId);
            return ApplicationErrors.PersonNotFound;
        }

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

        person.AssignDoctor(doctor);


        _context.People.Update(person);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Doctor {DoctorId} and Person {PersonId} updated successfully.", doctor.Id, person.Id);
        return Result.Updated;
    }
}
