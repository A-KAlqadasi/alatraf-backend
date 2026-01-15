using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.MedicalPrograms;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramById;

public class GetMedicalProgramByIdQueryHandler : IRequestHandler<GetMedicalProgramByIdQuery, Result<MedicalProgramDto>>
{
    private readonly ILogger<GetMedicalProgramByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetMedicalProgramByIdQueryHandler(ILogger<GetMedicalProgramByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<MedicalProgramDto>> Handle(GetMedicalProgramByIdQuery query, CancellationToken ct)
    {
        var medicalProgram = await _context.MedicalPrograms
        .Include(m=> m.Section)
        .FirstOrDefaultAsync(mp => mp.Id == query.MedicalProgramId, ct);
        
        if (medicalProgram is null)
        {
            _logger.LogWarning("Medical program not found: {MedicalProgramId}", query.MedicalProgramId);

            return MedicalProgramErrors.MedicalProgramNotFound;
        }
        
        return medicalProgram.ToDto();
    }
}