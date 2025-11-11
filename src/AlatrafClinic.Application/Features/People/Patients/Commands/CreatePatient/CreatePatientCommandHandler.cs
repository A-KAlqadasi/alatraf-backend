using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler(
    IUnitOfWork unitWork,
ILogger<CreatePatientCommandHandler> logger,
IPersonCreateService personCreateService,
    ICacheService cache // 👈 inject the cache

) : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IUnitOfWork _unitWork = unitWork;
    private readonly ILogger<CreatePatientCommandHandler> _logger = logger;
    private readonly IPersonCreateService _personCreateService = personCreateService;
    private readonly ICacheService _cache = cache;

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken ct)
    {
        var personResult = await _personCreateService.CreateAsync(request.Person, ct);

        if (personResult.IsError)
            return personResult.Errors;
        var person = personResult.Value;

        var patientResult = Patient.Create(
            personId: person.Id,
            patientType: request.PatientType
        // autoRegistrationNumber: request.AutoRegistrationNumber
        );

        if (patientResult.IsError)
            return patientResult.Errors;

        var patient = patientResult.Value;

        await _unitWork.Person.AddAsync(person, ct);
        await _unitWork.Patients.AddAsync(patient, ct);
        await _unitWork.SaveChangesAsync(ct);
        _logger.LogInformation("Patient created successfully with ID: {patient}", patient.Id);
        await _cache.RemoveByTagAsync("patient", ct);

        return patient.ToDto();
    }
}
