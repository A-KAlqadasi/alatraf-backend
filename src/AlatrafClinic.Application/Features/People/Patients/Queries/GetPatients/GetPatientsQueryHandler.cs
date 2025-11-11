using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Patients.Dtos;
using AlatrafClinic.Application.Features.People.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.People.Patients.Queries.GetPatients;

public class GetPatientsQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetPatientsQuery, Result<List<PatientDto>>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<List<PatientDto>>> Handle(GetPatientsQuery query, CancellationToken ct)
    {
        var patientsQuery = await _unitOfWork.Patients.GetPatientsWithPersonQueryAsync();

        // Apply filters
        patientsQuery = ApplyFilters(patientsQuery, query);

        // Apply search term
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            patientsQuery = ApplySearch(patientsQuery, query.SearchTerm!);

        // Execute query
        var patients = await patientsQuery.ToListAsync(ct);

        return patients.ToDtos();
    }

    private static IQueryable<Domain.Patients.Patient> ApplyFilters(
        IQueryable<Domain.Patients.Patient> query,
        GetPatientsQuery q)
    {
        if (q.PatientType.HasValue)
            query = query.Where(p => p.PatientType == q.PatientType.Value);

        if (q.Gender.HasValue)
            query = query.Where(p => p.Person != null && p.Person.Gender == q.Gender.Value);

        return query;
    }

    private static IQueryable<Domain.Patients.Patient> ApplySearch(
        IQueryable<Domain.Patients.Patient> query,
        string term)
    {
        var lowered = term.Trim().ToLower();
        var pattern = $"%{lowered}%";

        return query.Where(p =>
            p.Person != null && (
                EF.Functions.Like(p.Person.FullName.ToLower(), pattern) ||
                EF.Functions.Like(p.Person.Phone.ToLower(), pattern) ||
                (p.Person.NationalNo != null && EF.Functions.Like(p.Person.NationalNo.ToLower(), pattern))
            ));
    }
}
