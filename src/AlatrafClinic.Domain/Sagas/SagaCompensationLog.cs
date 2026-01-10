// Domain/Sagas/SagaCompensationLog.cs
using AlatrafClinic.Domain.Common;

namespace AlatrafClinic.Domain.Sagas
{
    public class SagaCompensationLog : Entity<int>
    {
        public Guid SagaId { get; set; }
        public string Message { get; set; } = default!;
        public DateTime CompensatedAt { get; set; }
        public bool IsAutoCompensation { get; set; }
        public bool Success { get; set; } = true;
        public SagaState Saga { get; set; } = default!;
    }
}