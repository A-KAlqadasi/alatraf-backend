
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateDeliveryTime;

public class CreateDeliveryTimeCommandHandler : IRequestHandler<CreateDeliveryTimeCommand, Result<Created>>
{
    private readonly ILogger<CreateDeliveryTimeCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDeliveryTimeCommandHandler(ILogger<CreateDeliveryTimeCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Created>> Handle(CreateDeliveryTimeCommand command, CancellationToken ct)
    {
        RepairCard? repairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);

        if (repairCard is null)
        {
            _logger.LogError("Repair card {repairCardId} not found to create delivery", command.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        var result = repairCard.AssignDeliveryTime(command.DeliveryDate, command.Notes);
        if (result.IsError)
        {
            _logger.LogError("Failed to assign delivery time for repair card {repairCardId}: {error}", command.RepairCardId, string.Join(", ", result.Errors));
            return result.TopError;
        }
        
        await _unitOfWork.RepairCards.UpdateAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Delivery time created for repair card {repairCardId}", command.RepairCardId);

        return Result.Created;
    }
}