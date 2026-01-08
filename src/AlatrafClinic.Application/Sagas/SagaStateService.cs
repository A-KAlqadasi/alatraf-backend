// Application/Sagas/SagaStateService.cs
using System;
using System.Threading;
using System.Threading.Tasks;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Sagas;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Sagas
{
    public interface ISagaStateService
    {
        Task StartOrContinueSagaAsync(Guid sagaId, string sagaType, string initialStep, CancellationToken ct);
        Task RecordStepAsync(Guid sagaId, string stepName, CancellationToken ct);
        Task RecordStepSuccessAsync(Guid sagaId, string stepName, CancellationToken ct);
        Task RecordStepFailureAsync(Guid sagaId, string stepName, string failureReason, CancellationToken ct);
        Task MarkSagaCompletedAsync(Guid sagaId, CancellationToken ct);
        Task<SagaState> GetSagaStateAsync(Guid sagaId, CancellationToken ct);
    }

    public class SagaStateService : ISagaStateService
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<SagaStateService> _logger;

        public SagaStateService(IAppDbContext db, ILogger<SagaStateService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task StartOrContinueSagaAsync(Guid sagaId, string sagaType, string initialStep, CancellationToken ct)
        {
            var existing = await _db.SagaStates
                .FirstOrDefaultAsync(s => s.Id == sagaId, ct);

            if (existing == null)
            {
                var sagaState = SagaState.Create(sagaId, sagaType);
                await _db.SagaStates.AddAsync(sagaState, ct);
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("Started new saga {SagaId} of type {SagaType}", sagaId, sagaType);
            }
            else
            {
                _logger.LogInformation("Continuing existing saga {SagaId}", sagaId);
            }
        }

        public async Task RecordStepAsync(Guid sagaId, string stepName, CancellationToken ct)
        {
            var saga = await _db.SagaStates
                .FirstOrDefaultAsync(s => s.Id == sagaId, ct);

            if (saga != null)
            {
                saga.RecordStep(stepName, false, "In progress");
                await _db.SaveChangesAsync(ct);
                _logger.LogDebug("Recorded step {Step} for saga {SagaId}", stepName, sagaId);
            }
        }

        public async Task RecordStepSuccessAsync(Guid sagaId, string stepName, CancellationToken ct)
        {
            var saga = await _db.SagaStates
                .Include(s => s.StepRecords)
                .FirstOrDefaultAsync(s => s.Id == sagaId, ct);

            if (saga != null)
            {
                // تحديث السجل الحالي أو إنشاء جديد
                var existingRecord = saga.StepRecords.LastOrDefault(r => r.StepName == stepName);
                if (existingRecord != null)
                {
                    existingRecord.Success = true;
                    existingRecord.Details = "Completed successfully";
                }
                else
                {
                    saga.RecordStep(stepName, true, "Completed successfully");
                }

                await _db.SaveChangesAsync(ct);
                _logger.LogDebug("Recorded successful step {Step} for saga {SagaId}", stepName, sagaId);
            }
        }

        public async Task RecordStepFailureAsync(Guid sagaId, string stepName, string failureReason, CancellationToken ct)
        {
            var saga = await _db.SagaStates
                .Include(s => s.StepRecords)
                .FirstOrDefaultAsync(s => s.Id == sagaId, ct);

            if (saga != null)
            {
                saga.RecordStep(stepName, false, failureReason);
                await _db.SaveChangesAsync(ct);
                _logger.LogWarning("Recorded failed step {Step} for saga {SagaId}: {Reason}",
                    stepName, sagaId, failureReason);
            }
        }

        public async Task MarkSagaCompletedAsync(Guid sagaId, CancellationToken ct)
        {
            var saga = await _db.SagaStates
                .FirstOrDefaultAsync(s => s.Id == sagaId, ct);

            if (saga != null)
            {
                saga.MarkCompleted();
                await _db.SaveChangesAsync(ct);
                _logger.LogInformation("Marked saga {SagaId} as completed", sagaId);
            }
        }

        public async Task<SagaState?> GetSagaStateAsync(Guid sagaId, CancellationToken ct)
        {
            return await _db.SagaStates
                .Include(s => s.StepRecords)
                .FirstOrDefaultAsync(s => s.Id == sagaId, ct);
        }
    }
}