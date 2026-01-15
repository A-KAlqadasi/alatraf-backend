using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;
using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Domain.Orders;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCardOrder;

public sealed class CreateRepairCardOrderCommandHandler : IRequestHandler<CreateRepairCardOrderCommand, Result<OrderDto>>
{
    private readonly ILogger<CreateRepairCardOrderCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRepairCardOrderCommandHandler(ILogger<CreateRepairCardOrderCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(CreateRepairCardOrderCommand command, CancellationToken ct)
    {
        // Load RepairCard aggregate
        var repairCard = await _unitOfWork.RepairCards.GetByIdAsync(command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("RepairCard with Id {RepairCardId} not found.", command.RepairCardId);
            return RepairCardErrors.InvalidDiagnosisId; // Use a generic not found/invalid error from domain
        }

        // Validate Section exists
        var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, ct);
        if (section is null)
        {
            _logger.LogError("Section with Id {SectionId} not found.", command.SectionId);
            return ApplicationErrors.SectionNotFound;
        }

        // Use domain factory to create the order instance
        var orderResult = Order.CreateForRepairCard(command.SectionId, command.RepairCardId);
        if (orderResult.IsError)
        {
            _logger.LogError("Failed to create Order domain object: {Errors}", orderResult.Errors);
            return orderResult.Errors;
        }

        var order = orderResult.Value;

        // Assign to repair card via domain method (maintain aggregate boundary)
        var assignResult = repairCard.AssignOrder(order);
        if (assignResult.IsError)
        {
            _logger.LogError("Failed to assign Order to RepairCard {RepairCardId}: {Errors}", command.RepairCardId, assignResult.Errors);
            return assignResult.Errors;
        }

        // Persist via RepairCards repository
        await _unitOfWork.RepairCards.UpdateAsync(repairCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} assigned to RepairCard {RepairCardId} successfully.", order.Id, command.RepairCardId);

        return order.ToDto();
    }
}
