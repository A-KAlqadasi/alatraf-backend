using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Commands.UpdatePatient;

public class UpdatePatientCommandHandler(
    IUnitWork unitWork
) : IRequestHandler<UpdatePatientCommand, Result<Updated>>
{
    private readonly IUnitWork _unitWork = unitWork;

    public async Task<Result<Updated>> Handle(UpdatePatientCommand request, CancellationToken ct)
    {
        var patient = await _unitWork.Patients.GetByIdAsync(request.PatientId, ct);
        if (patient is null)
            return ApplicationErrors.PatientNotFound

            ;

        var updateResult = patient.Update(patient.PersonId, request.PatientType);
        if (updateResult.IsError)
            return updateResult.Errors;


        await _unitWork.Patients.UpdateAsync(patient, ct);
        await _unitWork.SaveChangesAsync(ct);

        return Result.Updated;
    }
}
