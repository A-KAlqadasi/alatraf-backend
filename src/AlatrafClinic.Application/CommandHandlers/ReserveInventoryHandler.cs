// using System.Threading;
// using System.Threading.Tasks;

// using AlatrafClinic.Application.Commands;
// using AlatrafClinic.Application.Sagas;

// using MediatR;

// namespace AlatrafClinic.Application.CommandHandlers;

// public sealed class ReserveInventoryHandler : IRequestHandler<ReserveInventoryCommand, SaleSagaResult>
// {
//     private readonly SaleSagaOrchestrator _orchestrator;

//     public ReserveInventoryHandler(SaleSagaOrchestrator orchestrator)
//     {
//         _orchestrator = orchestrator;
//     }

//     public Task<SaleSagaResult> Handle(ReserveInventoryCommand request, CancellationToken cancellationToken)
//         => _orchestrator.ReserveInventoryAsync(request, cancellationToken);
// }
