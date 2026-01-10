using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.Orders;

using MediatR;
using AlatrafClinic.Application.Common.Errors;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCardOrder;

public sealed class CreateRepairCardOrderCommandHandler : IRequestHandler<CreateRepairCardOrderCommand, Result<OrderDto>>
{
    private readonly ILogger<CreateRepairCardOrderCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public CreateRepairCardOrderCommandHandler(ILogger<CreateRepairCardOrderCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<OrderDto>> Handle(CreateRepairCardOrderCommand command, CancellationToken ct)
    {
        // Load RepairCard aggregate (with orders to maintain invariants)
        var repairCard = await _dbContext.RepairCards
            .Include(rc => rc.Orders)
            .SingleOrDefaultAsync(rc => rc.Id == command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("RepairCard with Id {RepairCardId} not found.", command.RepairCardId);
            return RepairCardErrors.InvalidDiagnosisId; // Use a generic not found/invalid error from domain
        }

        // Validate Section exists
        var sectionExists = await _dbContext.Sections.AsNoTracking().AnyAsync(s => s.Id == command.SectionId, ct);
        if (!sectionExists)
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

        // Persist via DbContext
        _dbContext.RepairCards.Update(repairCard);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} assigned to RepairCard {RepairCardId} successfully.", order.Id, command.RepairCardId);

        return order.ToDto();
    }
}
