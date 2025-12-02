using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatients;

public class GetPatientsQueryHandler
    : IRequestHandler<GetPatientsQuery, Result<PaginatedList<PatientDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPatientsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<PatientDto>>> Handle(
        GetPatientsQuery query,
        CancellationToken ct)
    {
        var specification = new PatientsFilter(query);

        var totalCount = await _unitOfWork.Patients.CountAsync(specification, ct);

        var patients = await _unitOfWork.Patients
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var items = patients
            .Select(p => p.ToDto()) // assumes you have an extension Patient -> PatientDto
            .ToList();

        return new PaginatedList<PatientDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };
    }
}
