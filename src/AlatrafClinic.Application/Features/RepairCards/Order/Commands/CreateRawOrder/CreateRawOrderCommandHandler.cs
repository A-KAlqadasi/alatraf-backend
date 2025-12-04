using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.Orders;
using MechanicShop.Application.Common.Errors;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateRawOrder;

public sealed class CreateRawOrderCommandHandler : IRequestHandler<CreateRawOrderCommand, Result<OrderDto>>
{
    private readonly ILogger<CreateRawOrderCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRawOrderCommandHandler(ILogger<CreateRawOrderCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(CreateRawOrderCommand command, CancellationToken ct)
    {
        // Validate the referenced Section exists
        var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, ct);
        if (section is null)
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

        // Persist via Orders repository (Order is treated as an aggregate root)
        await _unitOfWork.Orders.AddAsync(order, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Raw Order with Id {OrderId} created successfully.", order.Id);

        return order.ToDto();
    }
}
