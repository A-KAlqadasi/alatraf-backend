using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateOrderSection;

public sealed class UpdateOrderSectionCommandHandler : IRequestHandler<UpdateOrderSectionCommand, Result<Updated>>
{
    private readonly ILogger<UpdateOrderSectionCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateOrderSectionCommandHandler(ILogger<UpdateOrderSectionCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Updated>> Handle(UpdateOrderSectionCommand command, CancellationToken ct)
    {
        var repairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("RepairCard with Id {RepairCardId} not found.", command.RepairCardId);
            return RepairCardErrors.InvalidDiagnosisId;
        }

        // Validate Section existence
        var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, ct);
        if (section is null)
        {
            _logger.LogError("Section with Id {SectionId} not found.", command.SectionId);
            return MechanicShop.Application.Common.Errors.ApplicationErrors.SectionNotFound;
        }

        var result = repairCard.UpdateOrderSection(command.OrderId, command.SectionId);
        if (result.IsError)
        {
            _logger.LogError("Failed to update order section for RepairCard {RepairCardId}, Order {OrderId}: {Errors}", command.RepairCardId, command.OrderId, result.Errors);
            return result.Errors;
        }

        await _unitOfWork.RepairCards.UpdateAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} for RepairCard {RepairCardId} updated to Section {SectionId}.", command.OrderId, command.RepairCardId, command.SectionId);

        return Result.Updated;
    }
}
