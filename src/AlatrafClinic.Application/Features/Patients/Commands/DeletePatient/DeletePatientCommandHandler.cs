
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteItemCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public DeletePatientCommandHandler(ILogger<DeleteItemCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeletePatientCommand command, CancellationToken ct)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(command.PatientId);

        if(patient is null)
        {
            _logger.LogError("Patient with Id {paitentid} is not found", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        await _unitOfWork.Patients.DeleteAsync(patient, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _cache.RemoveByTagAsync("patient", ct);

        return Result.Deleted;
    }
}