using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Services.UpdatePerson;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler(
    IPersonUpdateService personUpdateService,
    IAppDbContext context,
    ILogger<UpdatePatientCommandHandler> logger
) : IRequestHandler<UpdatePatientCommand, Result<Updated>>
{
    private readonly IPersonUpdateService _personUpdateService = personUpdateService;
    private readonly IAppDbContext _context = context;
    private readonly ILogger<UpdatePatientCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(UpdatePatientCommand command, CancellationToken ct)
    {
        var patient = await _context.Patients.FindAsync(new object[] { command.PatientId }, ct);
        if (patient is null)
        {
            _logger.LogWarning("Patient with ID {PatientId} not found.", command.PatientId);
            return ApplicationErrors.PatientNotFound;
        }

        var person = await _context.People.FindAsync(new object[] { patient.PersonId }, ct);
        if (person is null)
        {
            _logger.LogWarning("Person for Patient {PatientId} not found.", command.PatientId);
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

        var patientUpdate = patient.Update(patient.PersonId, command.PatientType);
        
        if (patientUpdate.IsError)
            return patientUpdate.Errors;

        person.AssignPatient(patient);

        _context.People.Update(person);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Patient {PatientId} and Person {PersonId} updated successfully.", patient.Id, person.Id);

        return Result.Updated;
    }
}