using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.DeleteMedicalProgram;

public class DeleteMedicalProgramCommandHandler : IRequestHandler<DeleteMedicalProgramCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteMedicalProgramCommandHandler> _logger;
    private readonly HybridCache _cache;

    public DeleteMedicalProgramCommandHandler(
        IUnitOfWork unitOfWork,
        ILogger<DeleteMedicalProgramCommandHandler> logger,
        HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _cache = cache;
    }

    public async Task<Result<Deleted>> Handle(DeleteMedicalProgramCommand command, CancellationToken ct)
    {
        var medicalProgram = await _unitOfWork.MedicalPrograms.GetByIdAsync(command.MedicalProgramId, ct);
        if (medicalProgram is null)
        {
            _logger.LogWarning("Medical program with ID {MedicalProgramId} not found.", command.MedicalProgramId);
            return MedicalProgramErrors.MedicalProgramNotFound;
        }

        await _unitOfWork.MedicalPrograms.DeleteAsync(medicalProgram, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result.Deleted;
    }
}