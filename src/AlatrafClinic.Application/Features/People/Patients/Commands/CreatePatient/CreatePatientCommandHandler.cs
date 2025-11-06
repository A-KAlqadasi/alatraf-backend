using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.CreatePatient;

public class CreatePatientCommandHandler(
    IUnitOfWork unitWork
) : IRequestHandler<CreatePatientCommand, Result<PatientDto>>
{
    private readonly IUnitOfWork _unitWork = unitWork;

    public async Task<Result<PatientDto>> Handle(CreatePatientCommand request, CancellationToken ct)
    {
        var person = await _unitWork.Person.GetByIdAsync(request.PersonId, ct);
        if (person is null)
            return ApplicationErrors.PersonNotFound;

        var isPatient = await _unitWork.Patients.GetByPersonIdAsync(request.PersonId, ct);
        if (isPatient is not null)
            return ApplicationErrors.PatientAlreadyExists(request.PersonId);

        var isDoctor = await _unitWork.Doctors.GetByPersonIdAsync(request.PersonId, ct);
        if (isDoctor is not null)
            return ApplicationErrors.PersonAlreadyAssigned(request.PersonId);

        var isEmployee = await _unitWork.Employees.GetByPersonIdAsync(request.PersonId, ct);
        if (isEmployee is not null)
            return ApplicationErrors.PersonAlreadyAssigned(request.PersonId);

        var patientResult = Patient.Create(
            personId: request.PersonId,
            patientType: request.PatientType
            // autoRegistrationNumber: request.AutoRegistrationNumber
        );

        if (patientResult.IsError)
            return patientResult.Errors;

        var patient = patientResult.Value;

        await _unitWork.Patients.AddAsync(patient, ct);
        await _unitWork.SaveChangesAsync(ct);

        return patient.ToDto();
    }
}