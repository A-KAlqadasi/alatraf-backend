// Application/Sagas/Mappers/SagaStateMapper.cs - تحديث كامل
using System.Linq;
using AlatrafClinic.Application.Sagas.Dtos;
using AlatrafClinic.Domain.Sagas;

namespace AlatrafClinic.Application.Sagas.Mappers
{
    public static class SagaStateMapper
    {
        public static SagaStateDto ToDto(this SagaState sagaState)
        {
            if (sagaState == null)
                return null!;

            return new SagaStateDto
            {
                SagaId = sagaState.Id,
                SagaType = sagaState.SagaType,
                Status = sagaState.Status.ToString(),
                CurrentStep = sagaState.CurrentStep,
                StartedAt = sagaState.StartedAt,
                CompletedAt = sagaState.CompletedAt,
                FailedAt = sagaState.FailedAt,
                FailureReason = sagaState.FailureReason,
                IsCompensating = sagaState.IsCompensating,
                RetryCount = sagaState.RetryCount,
                LastRetryAt = sagaState.LastRetryAt,
                LastError = sagaState.LastError,
                IsAutoCompensated = sagaState.IsAutoCompensated,
                AutoCompensatedAt = sagaState.AutoCompensatedAt,
                StepRecords = sagaState.StepRecords.Select(r => r.ToDto()).ToList()
            };
        }

        public static SagaStepRecordDto ToDto(this SagaStepRecord stepRecord)
        {
            if (stepRecord == null)
                return null!;

            return new SagaStepRecordDto
            {
                StepName = stepRecord.StepName,
                ExecutedAt = stepRecord.ExecutedAt,
                Success = stepRecord.Success,
                Details = stepRecord.Details
            };
        }
    }
}