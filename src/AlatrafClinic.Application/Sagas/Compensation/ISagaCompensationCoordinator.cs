// // Application/Sagas/Compensation/ISagaCompensationCoordinator.cs

// using AlatrafClinic.Application.Sagas.Compensation;

// namespace AlatrafClinic.Application.Sagas.Compensation
// {
//     public interface ISagaCompensationCoordinator
//     {
//         Task<CompensationResult> ExecuteCompensationAsync(Guid sagaId, CancellationToken ct);
//         Task<CompensationResult> RetryFailedSagaAsync(Guid sagaId, CancellationToken ct);
//     }
// }