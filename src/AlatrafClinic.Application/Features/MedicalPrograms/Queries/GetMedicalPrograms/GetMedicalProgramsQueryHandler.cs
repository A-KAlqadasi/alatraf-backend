using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalPrograms;

public class GetMedicalProgramsQueryHandler
    : IRequestHandler<GetMedicalProgramsQuery, Result<List<MedicalProgramDto>>>
{
    private readonly IAppDbContext _context;

    public GetMedicalProgramsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<MedicalProgramDto>>> Handle(
        GetMedicalProgramsQuery query,
        CancellationToken ct)
    {
        var programs = await _context.MedicalPrograms.ToListAsync(ct);

        return programs.ToDtos();
    }
}