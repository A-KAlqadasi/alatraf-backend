using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Patients.Queries.GetPatientById;

public class GetPatientByIdQueryHandler(
    IAppDbContext context
) : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
{
    private readonly IAppDbContext _context = context;

    public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery query, CancellationToken ct)
    {
        var patient = await _context.Patients.Include(p=> p.Person).FirstOrDefaultAsync(p=> p.Id == query.PatientId, ct);
        if (patient is null)
        {
            return ApplicationErrors.PatientNotFound;
        }
        
        return patient.ToDto();
    }
}
