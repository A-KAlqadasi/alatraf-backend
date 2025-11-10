using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;


namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.UpdateMedicalProgram;

public class UpdateMedicalProgramCommandHandler : IRequestHandler<UpdateMedicalProgramCommand, Result<Updated>>
{
    private readonly ILogger<UpdateMedicalProgramCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public UpdateMedicalProgramCommandHandler(ILogger<UpdateMedicalProgramCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<Updated>> Handle(UpdateMedicalProgramCommand command, CancellationToken ct)
    {
        var medicalProgram = await _unitOfWork.MedicalPrograms.GetByIdAsync(command.MedicalProgramId, ct);

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

        await _unitOfWork.MedicalPrograms.UpdateAsync(medicalProgram, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Updated;
    }
}