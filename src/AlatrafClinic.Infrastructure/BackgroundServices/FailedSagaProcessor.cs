// Infrastructure/BackgroundServices/FailedSagaProcessor.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Sagas.Compensation;
using AlatrafClinic.Domain.Sagas;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Infrastructure.BackgroundServices
{
    public class FailedSagaProcessor : BackgroundService
    {
        private readonly ILogger<FailedSagaProcessor> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public FailedSagaProcessor(
            ILogger<FailedSagaProcessor> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Failed Saga Processor is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessFailedSagasAsync(stoppingToken);
                    await Task.Delay(_interval, stoppingToken);
                }
                catch (Exception ex) when (ex is not TaskCanceledException)
                {
                    _logger.LogError(ex, "Error in Failed Saga Processor");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }

            _logger.LogInformation("Failed Saga Processor is stopping.");
        }

        private async Task ProcessFailedSagasAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<IAppDbContext>();
            var compensationCoordinator = scope.ServiceProvider.GetRequiredService<ISagaCompensationCoordinator>();

            var failedSagas = await db.SagaStates
                .Where(s => s.Status == SagaStatus.Failed && !s.IsAutoCompensated)
                .OrderBy(s => s.FailedAt)
                .Take(10)
                .ToListAsync(ct);

            if (!failedSagas.Any())
            {
                _logger.LogDebug("No failed sagas to process");
                return;
            }

            _logger.LogInformation("Processing {Count} failed sagas", failedSagas.Count);

            foreach (var saga in failedSagas)
            {
                try
                {
                    _logger.LogInformation("Processing failed saga {SagaId}", saga.Id);

                    var result = await compensationCoordinator.ExecuteCompensationAsync(saga.Id, ct);

                    if (result.Success)
                    {
                        saga.MarkAutoCompensated();
                        await db.SaveChangesAsync(ct);
                        _logger.LogInformation("Successfully auto-compensated saga {SagaId}", saga.Id);
                    }
                    else
                    {
                        saga.IncrementRetryCount();
                        await db.SaveChangesAsync(ct);
                        _logger.LogWarning("Failed to auto-compensate saga {SagaId}: {Errors}",
                            saga.Id, string.Join(", ", result.Errors));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing failed saga {SagaId}", saga.Id);
                    //saga.LastError = ex.Message;
                    saga.IncrementRetryCount();
                    await db.SaveChangesAsync(ct);
                }
            }
        }
    }
}