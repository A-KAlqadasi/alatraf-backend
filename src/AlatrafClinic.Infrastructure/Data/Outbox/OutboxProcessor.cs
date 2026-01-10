using System.Text.Json;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Infrastructure.Data;
using AlatrafClinic.Application.Common.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public sealed class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxProcessor> _logger;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Allow the app to finish starting before attempting outbox work
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingMessagesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Host is stopping; exit gracefully
                break;
            }
            catch (ObjectDisposedException) when (stoppingToken.IsCancellationRequested)
            {
                // Service provider is disposing due to shutdown; exit gracefully
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processor failed; retrying after delay.");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }

    private async Task ProcessPendingMessagesAsync(CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var db = scope.ServiceProvider
            .GetRequiredService<AlatrafClinicDbContext>();

        var messagePublisher = scope.ServiceProvider
            .GetRequiredService<AlatrafClinic.Application.Common.Interfaces.Messaging.IMessagePublisher>();

        var eventContext = scope.ServiceProvider
            .GetRequiredService<AlatrafClinic.Application.Common.Events.IEventContext>();

        var messages = await db.OutboxMessages
            .Where(x => x.ProcessedOnUtc == null)
            .OrderBy(x => x.OccurredOnUtc)
            .Take(20)
            .ToListAsync(stoppingToken);

        foreach (var message in messages)
        {
            var type = Type.GetType(message.Type);
            if (type is null)
            {
                _logger.LogWarning("Outbox message type {TypeName} could not be resolved. Skipping.", message.Type);
                continue;
            }

            // Only process integration events
            if (!typeof(AlatrafClinic.Application.Common.Events.IIntegrationEvent).IsAssignableFrom(type))
            {
                _logger.LogDebug("Outbox message {Id} is not an IIntegrationEvent. Skipping.", message.Id);
                message.ProcessedOnUtc = DateTime.UtcNow;
                continue;
            }

            var integrationEvent = JsonSerializer.Deserialize(message.Content, type);
            if (integrationEvent is null)
            {
                _logger.LogWarning("Failed to deserialize outbox message {Id} to {Type}", message.Id, message.Type);
                continue;
            }

            try
            {
                eventContext.CurrentMessageId = message.Id;
                await messagePublisher.PublishAsync(integrationEvent, stoppingToken);
                message.ProcessedOnUtc = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish outbox message {MessageId}", message.Id);
            }
            finally
            {
                eventContext.CurrentMessageId = null;
            }
        }

        await db.SaveChangesAsync(stoppingToken);
    }
}
