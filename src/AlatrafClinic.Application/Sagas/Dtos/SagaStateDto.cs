// Application/Sagas/Dtos/SagaStateDto.cs
using System;
using System.Collections.Generic;

namespace AlatrafClinic.Application.Sagas.Dtos
{
    public class SagaStateDto
    {
        public Guid SagaId { get; set; }
        public string SagaType { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string CurrentStep { get; set; } = default!;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public string? FailureReason { get; set; }
        public bool IsCompensating { get; set; }
        public int RetryCount { get; set; }
        public DateTime? LastRetryAt { get; set; }
        public string? LastError { get; set; }
        public bool IsAutoCompensated { get; set; }
        public DateTime? AutoCompensatedAt { get; set; }
        public List<SagaStepRecordDto> StepRecords { get; set; } = new();
    }

    public class SagaStepRecordDto
    {
        public string StepName { get; set; } = default!;
        public DateTime ExecutedAt { get; set; }
        public bool Success { get; set; }
        public string? Details { get; set; }
    }
}