using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Persons.Services.UpdatePerson;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler(
    IPersonUpdateService personUpdateService,
    IUnitOfWork unitWork,
    ILogger<UpdatePatientCommandHandler> logger,

    ICacheService cache

) : IRequestHandler<UpdatePatientCommand, Result<Updated>>
{
    private readonly IPersonUpdateService _personUpdateService = personUpdateService;
    private readonly IUnitOfWork _unitWork = unitWork;
    private readonly ILogger<UpdatePatientCommandHandler> _logger = logger;
    private readonly ICacheService _cache = cache;

    public async Task<Result<Updated>> Handle(UpdatePatientCommand request, CancellationToken ct)
    {
        var patient = await _unitWork.Patients.GetByIdAsync(request.PatientId, ct);
        if (patient is null)
        {
            _logger.LogWarning("Patient with ID {PatientId} not found.", request.PatientId);
            return ApplicationErrors.PatientNotFound;
        }

        var person = await _unitWork.Person.GetByIdAsync(patient.PersonId, ct);
        if (person is null)
        {
            _logger.LogWarning("Person for Patient {PatientId} not found.", request.PatientId);
            return ApplicationErrors.PersonNotFound;
        }

        var personUpdate = await _personUpdateService.UpdateAsync(person.Id, request.Person, ct);
        if (personUpdate.IsError)
            return personUpdate.Errors;

        var patientUpdate = patient.Update(patient.PersonId, request.PatientType);
        if (patientUpdate.IsError)
            return patientUpdate.Errors;

        await _unitWork.Person.UpdateAsync(person, ct);
        await _unitWork.Patients.UpdateAsync(patient, ct);
        await _unitWork.SaveChangesAsync(ct);

        _logger.LogInformation("Patient {PatientId} and Person {PersonId} updated successfully.", patient.Id, person.Id);

        await _cache.RemoveByTagAsync("patient", ct);

        return Result.Updated;
    }
}
