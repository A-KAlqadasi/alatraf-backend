using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.IndustrialParts.Commands.DeleteIndustrialPart;

public class DeleteIndustrialPartCommandHandler : IRequestHandler<DeleteIndustrialPartCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteIndustrialPartCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteIndustrialPartCommandHandler(ILogger<DeleteIndustrialPartCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Deleted>> Handle(DeleteIndustrialPartCommand command, CancellationToken ct)
    {
        var industrialPart = await _unitOfWork.IndustrialParts.GetByIdAsync(command.IndustrialPartId, ct);
        if (industrialPart is null)
        {
            _logger.LogError("Industrial Part with Id {IndustrialPartId} not found.", command.IndustrialPartId);
            return Result.Deleted;
        }

        await _unitOfWork.IndustrialParts.DeleteAsync(industrialPart, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Industrial Part with Id {IndustrialPartId} deleted successfully.", command.IndustrialPartId);
        return Result.Deleted;
    }
}