using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.DeleteMedicalProgram;

public class DeleteMedicalProgramCommandHandler : IRequestHandler<DeleteMedicalProgramCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteMedicalProgramCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IAppDbContext _context;

    public DeleteMedicalProgramCommandHandler(
        IAppDbContext context,
        ILogger<DeleteMedicalProgramCommandHandler> logger,
        HybridCache cache)
    {
        _context = context;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Deleted>> Handle(DeleteMedicalProgramCommand command, CancellationToken ct)
    {
        var medicalProgram = await _context.MedicalPrograms.FirstOrDefaultAsync(mp=> mp.Id == command.MedicalProgramId, ct);
        if (medicalProgram is null)
        {
            _logger.LogWarning("Medical program with ID {MedicalProgramId} not found.", command.MedicalProgramId);
            return MedicalProgramErrors.MedicalProgramNotFound;
        }

        _context.MedicalPrograms.Remove(medicalProgram);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("medical-program");

        _logger.LogInformation("Medical program with ID {MedicalProgramId} deleted successfully.", command.MedicalProgramId);

        return Result.Deleted;
    }
}