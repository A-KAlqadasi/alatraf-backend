
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler(
    IUnitWork unitWork
) : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IUnitWork _unitWork = unitWork;

    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery query, CancellationToken ct)
    {
        var patient = await _unitWork.Patients.GetByIdWithPersonAsync(query.PatientId, ct);

        if (patient is null)
            return ApplicationErrors.PatientNotFound;;
        return patient.ToDto();
    }
}
