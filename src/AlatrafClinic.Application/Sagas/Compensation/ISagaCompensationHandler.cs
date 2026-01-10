// Application/Sagas/Compensation/ISagaCompensationHandler.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AlatrafClinic.Application.Sagas.Compensation
{
    public interface ISagaCompensationHandler
    {
        string SagaType { get; }
        Task<CompensationResult> CompensateAsync(Guid sagaId, CancellationToken ct);
    }

    public record CompensationResult
    {
        public bool Success { get; init; }
        public List<string> Errors { get; init; } = new();

        
        public static CompensationResult SuccessResult() => new() { Success = true };
        public static CompensationResult Failure(params string[] errors) =>
            new() { Success = false, Errors = new List<string>(errors) };
    }

    public interface ISagaCompensationCoordinator
    {
        Task<CompensationResult> ExecuteCompensationAsync(
            Guid sagaId,
            CancellationToken ct);

        Task<CompensationResult> RetryFailedSagaAsync(Guid sagaId, CancellationToken ct);
    }
}
