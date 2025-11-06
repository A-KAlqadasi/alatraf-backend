using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetPatients;

public class GetPatientsQueryHandler(
    IUnitOfWork unitWork
) : IRequestHandler<GetPatientsQuery, Result<List<PatientDto>>>
{
    private readonly IUnitOfWork _unitWork = unitWork;

    public async Task<Result<List<PatientDto>>> Handle(GetPatientsQuery query, CancellationToken ct)
    {
        var patients = await _unitWork.Patients.GetAllWithPersonAsync(ct);


        return patients.ToDtos();
    }
}
