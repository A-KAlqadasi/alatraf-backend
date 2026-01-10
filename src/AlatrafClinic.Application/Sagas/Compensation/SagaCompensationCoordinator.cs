// Application/Sagas/Compensation/SagaCompensationCoordinator.cs
using AlatrafClinic.Application.Common.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Sagas.Compensation
{
    public class SagaCompensationCoordinator : ISagaCompensationCoordinator
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<SagaCompensationCoordinator> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SagaCompensationCoordinator(
            IAppDbContext db,
            ILogger<SagaCompensationCoordinator> logger,
            IServiceProvider serviceProvider)
        {
            _db = db;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task<CompensationResult> ExecuteCompensationAsync(Guid sagaId, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("Starting compensation for saga {SagaId}", sagaId);

                var sagaState = await _db.SagaStates
                    .Include(s => s.StepRecords)
                    .FirstOrDefaultAsync(s => s.Id == sagaId, ct);

                if (sagaState == null)
                {
                    return  CompensationResult.Failure($"Saga {sagaId} not found");
                }

                sagaState.StartCompensation();
                await _db.SaveChangesAsync(ct);

                var compensationHandlers = _serviceProvider.GetServices<ISagaCompensationHandler>();
                var handler = compensationHandlers.FirstOrDefault(h => h.SagaType == sagaState.SagaType);

                if (handler == null)
                {
                    return  CompensationResult.Failure($"No compensation handler found for {sagaState.SagaType}");
                }

                var result = await handler.CompensateAsync(sagaId, ct);

                if (result.Success)
                {
                    sagaState.MarkCompensated();
                    await _db.SaveChangesAsync(ct);
                    _logger.LogInformation("Compensation completed successfully for saga {SagaId}", sagaId);
                }
                else
                {
                    _logger.LogError("Compensation failed for saga {SagaId}: {Errors}", sagaId, string.Join(", ", result.Errors));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during compensation for saga {SagaId}", sagaId);
                return  CompensationResult.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<CompensationResult> RetryFailedSagaAsync(Guid sagaId, CancellationToken ct)
        {
            _logger.LogInformation("Retrying failed saga {SagaId}", sagaId);

            // يجب أن تنفذ هذا المنطق وفقاً لمتطلباتك
            return  CompensationResult.Failure("Retry functionality not implemented yet");
        }
    }
}