using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalPrograms;

public class GetMedicalProgramsQueryHandler : IRequestHandler<GetMedicalProgramsQuery, Result<List<MedicalProgramDto>>>
{
    private readonly IAppDbContext _context;

    public GetMedicalProgramsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<MedicalProgramDto>>> Handle(GetMedicalProgramsQuery query, CancellationToken ct)
    {
        var medicalPrograms = await _context.MedicalPrograms.ToListAsync(ct);
        if (medicalPrograms is null || !medicalPrograms.Any())
        {
           return Error.NotFound("Medical programs not found.");
        }
        return medicalPrograms.ToDtos();
    }
}