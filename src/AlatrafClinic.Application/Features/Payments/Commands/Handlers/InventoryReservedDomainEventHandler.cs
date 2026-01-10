using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Messaging;
using AlatrafClinic.Application.Common.Events;
using AlatrafClinic.Domain.Inventory.Reservations.Events;
using AlatrafClinic.Domain.Payments;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Payments.Handlers;

// Saga logic – to be migrated: currently creates payment reactively from domain event
// Trigger: InventoryReservedDomainEvent (domain)
// Side effects: loads sale+diagnosis, creates payment, saves, may cancel sale
// Persistence boundary: directly calls SaveChanges inside handler

public sealed class InventoryReservedDomainEventHandler
    : INotificationHandler<InventoryReservedDomainEvent>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<InventoryReservedDomainEventHandler> _logger;

    public InventoryReservedDomainEventHandler(
        IAppDbContext db,
        ILogger<InventoryReservedDomainEventHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task Handle(
        InventoryReservedDomainEvent notification,
        CancellationToken ct)
    {
        // Saga logic now handled in application orchestrator; handler kept as a passive notification
        _logger.LogInformation("InventoryReservedDomainEvent received for Sale {SaleId}; orchestration handled in saga.", notification.SaleId);
        return Task.CompletedTask;
    }
}
