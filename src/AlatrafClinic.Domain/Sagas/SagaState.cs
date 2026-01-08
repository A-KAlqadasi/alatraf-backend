// Domain/Sagas/SagaState.cs
using System;
using System.Collections.Generic;

using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Sagas
{
    public class SagaState : Entity<Guid>, IAggregateRoot
    {
        public string SagaType { get; private set; }
        public SagaStatus Status { get; private set; }
        public string CurrentStep { get; private set; }
        public DateTime StartedAt { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public DateTime? FailedAt { get; private set; }
        public string? FailureReason { get; private set; }
        public bool IsCompensating { get; private set; }
        public int RetryCount { get; private set; }
        public DateTime? LastRetryAt { get; private set; }
        public string? LastError { get; private set; }
        public bool IsAutoCompensated { get; private set; }
        public DateTime? AutoCompensatedAt { get; private set; }

        private readonly List<SagaStepRecord> _stepRecords = new();
        public IReadOnlyCollection<SagaStepRecord> StepRecords => _stepRecords.AsReadOnly();

        // Private constructor for EF Core
        private SagaState()
        {
            SagaType = default!;
            CurrentStep = default!;
        }

        public static SagaState Create(Guid sagaId, string sagaType)
        {
            return new SagaState
            {
                SagaType = sagaType,
                Status = SagaStatus.Started,
                CurrentStep = "Started",
                StartedAt = DateTime.UtcNow
            };
        }

        public void RecordStep(string stepName, bool success, string? details = null)
        {
            _stepRecords.Add(new SagaStepRecord
            {
                Id = Guid.NewGuid(),
                StepName = stepName,
                ExecutedAt = DateTime.UtcNow,
                Success = success,
                Details = details,
                SagaId = Id,
                Saga = this
            });

            CurrentStep = stepName;

            if (!success)
            {
                Status = SagaStatus.Failed;
                FailedAt = DateTime.UtcNow;
                FailureReason = details;
            }
            else if (Status == SagaStatus.Started)
            {
                Status = SagaStatus.InProgress;
            }
        }

        public void MarkCompleted()
        {
            Status = SagaStatus.Completed;
            CompletedAt = DateTime.UtcNow;
        }

        public void StartCompensation()
        {
            IsCompensating = true;
            Status = SagaStatus.Compensating;
        }

        public void MarkCompensated()
        {
            Status = SagaStatus.Compensated;
            IsCompensating = false;
        }

        public void IncrementRetryCount()
        {
            RetryCount++;
            LastRetryAt = DateTime.UtcNow;
        }

        public void MarkAutoCompensated()
        {
            IsAutoCompensated = true;
            AutoCompensatedAt = DateTime.UtcNow;
            Status = SagaStatus.Compensated;
        }

        public void RecordSagaFailure(string error)
        {
            Status = SagaStatus.Failed;
            FailedAt = DateTime.UtcNow;
            FailureReason = error;
            LastError = error;
        }

        // إزالة ToDto() تماماً من هنا
    }

    public class SagaStepRecord
    {
        public Guid Id { get; set; }
        public string StepName { get; set; } = default!;
        public DateTime ExecutedAt { get; set; }
        public bool Success { get; set; }
        public string? Details { get; set; }
        public Guid SagaId { get; set; }
        public SagaState Saga { get; set; } = default!;
    }

    public enum SagaStatus
    {
        Started = 1,
        InProgress = 2,
        Completed = 3,
        Failed = 4,
        Compensating = 5,
        Compensated = 6,
        CompensationFailed = 7
    }
}

