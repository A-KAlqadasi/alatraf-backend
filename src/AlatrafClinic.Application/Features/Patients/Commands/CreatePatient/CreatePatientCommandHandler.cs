using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Application.Features.People.Services.CreatePerson;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler(
IAppDbContext context,
ILogger<CreatePatientCommandHandler> logger,
IPersonCreateService personCreateService
) : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreatePatientCommandHandler> _logger = logger;
    private readonly IPersonCreateService _personCreateService = personCreateService;

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand command, CancellationToken ct)
    {
        var personResult = await _personCreateService.CreateAsync(
            command.Fullname,
            command.Birthdate,
            command.Phone,
            command.NationalNo,
            command.Address,
            command.Gender,
            ct);

        if (personResult.IsError)
        {
            return personResult.Errors;
        }

        var person = personResult.Value;

        var patientResult = Patient.Create(
            personId: person.Id,
            patientType: command.PatientType
        );

        if (patientResult.IsError)
        {
            return patientResult.Errors;
        }

        var patient = patientResult.Value;
        person.AssignPatient(patient);

        await _context.People.AddAsync(person, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Patient created successfully with ID: {patient}", patient.Id);

        return patient.ToDto();
    }
}
