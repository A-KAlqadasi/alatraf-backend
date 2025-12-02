using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler(
    IUnitOfWork unitWork
) : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IUnitOfWork _unitWork = unitWork;

    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery query, CancellationToken ct)
    {
        var patient = await _unitWork.Patients.GetByIdAsync(query.PatientId, ct);

        if (patient is null)
            return ApplicationErrors.PatientNotFound;
            
        return patient.ToDto();
    }
}
