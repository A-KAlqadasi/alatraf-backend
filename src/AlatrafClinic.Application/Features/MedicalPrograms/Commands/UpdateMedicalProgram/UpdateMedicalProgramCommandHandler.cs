using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.UpdateMedicalProgram;

public class UpdateMedicalProgramCommandHandler : IRequestHandler<UpdateMedicalProgramCommand, Result<Updated>>
{
    private readonly ILogger<UpdateMedicalProgramCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public UpdateMedicalProgramCommandHandler(ILogger<UpdateMedicalProgramCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateMedicalProgramCommand command, CancellationToken ct)
    {
        var medicalProgram = await _context.MedicalPrograms.FirstOrDefaultAsync(mp => mp.Id == command.MedicalProgramId, ct);

        if (medicalProgram is null)
        {
            _logger.LogWarning("Medical program with ID {MedicalProgramId} not found.", command.MedicalProgramId);
            return MedicalProgramErrors.MedicalProgramNotFound;
        }

        var updateResult = medicalProgram.Update(command.Name, command.Description, command.SectionId);

        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update medical program: {Error}", updateResult.Errors);
            return updateResult.TopError;
        }

        _context.MedicalPrograms.Update(medicalProgram);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("medical-program");

        _logger.LogInformation("Medical program with ID {MedicalProgramId} updated successfully.", command.MedicalProgramId);

        return Result.Updated;
    }
}