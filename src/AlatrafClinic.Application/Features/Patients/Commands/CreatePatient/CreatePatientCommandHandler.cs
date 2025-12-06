using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Application.Features.People.Services.CreatePerson;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler(
    IUnitOfWork unitWork,
ILogger<CreatePatientCommandHandler> logger,
IPersonCreateService personCreateService,
    HybridCache cache

) : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IUnitOfWork _unitWork = unitWork;
    private readonly ILogger<CreatePatientCommandHandler> _logger = logger;
    private readonly IPersonCreateService _personCreateService = personCreateService;
    private readonly HybridCache _cache = cache;

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

        await _unitWork.People.AddAsync(person, ct);
        await _unitWork.Patients.AddAsync(patient, ct);
        await _unitWork.SaveChangesAsync(ct);

        _logger.LogInformation("Patient created successfully with ID: {patient}", patient.Id);
        await _cache.RemoveByTagAsync("patient", ct);

        return patient.ToDto();
    }
}
