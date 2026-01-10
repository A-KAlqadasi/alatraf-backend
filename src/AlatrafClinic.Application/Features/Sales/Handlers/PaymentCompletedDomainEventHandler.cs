using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Messaging;
using AlatrafClinic.Application.Common.Events;
using AlatrafClinic.Domain.Payments.Events;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public sealed class PaymentCompletedDomainEventHandler
    : INotificationHandler<PaymentCompletedDomainEvent>
{
    private readonly IAppDbContext _db;
    private readonly ILogger<PaymentCompletedDomainEventHandler> _logger;

    public PaymentCompletedDomainEventHandler(IAppDbContext db, ILogger<PaymentCompletedDomainEventHandler> logger)
    {
        _db = db;
        _logger = logger;
    }

    public Task Handle(
        PaymentCompletedDomainEvent notification,
        CancellationToken ct)
    {
        // Saga logic now handled in application orchestrator; handler kept as passive notification
        _logger.LogInformation("PaymentCompletedDomainEvent received for Diagnosis {DiagnosisId}; orchestration handled in saga.", notification.DiagnosisId);
        return Task.CompletedTask;
    }
}
