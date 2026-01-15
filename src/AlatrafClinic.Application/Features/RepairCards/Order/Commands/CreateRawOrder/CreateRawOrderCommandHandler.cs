using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;
using AlatrafClinic.Application.Common.Errors;

using AlatrafClinic.Domain.Orders;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRawOrder;

public sealed class CreateRawOrderCommandHandler : IRequestHandler<CreateRawOrderCommand, Result<OrderDto>>
{
    private readonly ILogger<CreateRawOrderCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public CreateRawOrderCommandHandler(ILogger<CreateRawOrderCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<OrderDto>> Handle(CreateRawOrderCommand command, CancellationToken ct)
    {
        // Validate the referenced Section exists
        var sectionExists = await _dbContext.Sections.AsNoTracking().AnyAsync(s => s.Id == command.SectionId, ct);
        if (!sectionExists)
        {
            _logger.LogError("Section with Id {SectionId} not found.", command.SectionId);
            return ApplicationErrors.SectionNotFound;
        }

        var orderResult = Order.CreateForRaw(command.SectionId);
        if (orderResult.IsError)
        {
            _logger.LogError("Failed to create Order: {Errors}", orderResult.Errors);
            return orderResult.Errors;
        }

        var order = orderResult.Value;

        // Persist via DbContext (Order is treated as an aggregate root)
        await _dbContext.Orders.AddAsync(order, ct);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Raw Order with Id {OrderId} created successfully.", order.Id);

        return order.ToDto();
    }
}
