using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Inventory.Reservations;
using AlatrafClinic.Domain.Inventory.Reservations.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Inventory.Handlers;

public sealed class InventoryReservationFailedDomainEventHandler
    : INotificationHandler<InventoryReservationFailedDomainEvent>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<InventoryReservationFailedDomainEventHandler> _logger;

    public InventoryReservationFailedDomainEventHandler(
        IAppDbContext db,
        ILogger<InventoryReservationFailedDomainEventHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task Handle(
        InventoryReservationFailedDomainEvent notification,
        CancellationToken ct)
    {
        // Saga logic – to be migrated: handler no longer mutates inventory; orchestration handled in application layer
        _logger.LogInformation("InventoryReservationFailedDomainEvent received for Sale {SaleId}; handled by saga orchestrator.", notification.SaleId);
        await Task.CompletedTask;
    }
}
