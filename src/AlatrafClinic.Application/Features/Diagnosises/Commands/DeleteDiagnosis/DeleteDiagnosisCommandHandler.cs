using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.DeleteDiagnosis;

public class DeleteDiagnosisCommandHandler : IRequestHandler<DeleteDiagnosisCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteDiagnosisCommandHandler> _logger;
    private readonly IUnitOfWork _uow;
    private readonly HybridCache _cache;

    public DeleteDiagnosisCommandHandler(ILogger<DeleteDiagnosisCommandHandler> logger, IUnitOfWork uow, HybridCache cache)
    {
        _logger = logger;
        _uow = uow;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteDiagnosisCommand command, CancellationToken ct)
    {
        var diagnosis = await _uow.Diagnosises.GetByIdAsync(command.diagnosisId, ct);
        if (diagnosis is null)
        {
            _logger.LogWarning("Diagnosis with Id {DiagnosisId} not found.", command.diagnosisId);
            return Error.NotFound(code: "Diagnosis.NotFound", description: $"Diagnosis with Id {command.diagnosisId} not found for delete.");
        }

        if (await _uow.Diagnosises.HasAssociationsAsync(command.diagnosisId, ct))
        {
            _logger.LogWarning("Diagnosis with Id {DiagnosisId} has associations and cannot be deleted.", command.diagnosisId);
            return Error.Conflict(code: "Diagnosis.HasAssociations", description: $"Diagnosis with Id {command.diagnosisId} has associations and cannot be deleted.");
        }

        await _uow.Diagnosises.DeleteAsync(diagnosis, ct);
        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("Diagnosis with Id {DiagnosisId} deleted successfully.", command.diagnosisId);
        
        return Result.Deleted;
    }
}