// using System.Threading;
// using System.Threading.Tasks;

// using AlatrafClinic.Application.Commands;
// using AlatrafClinic.Application.Sagas;

// using MediatR;

// namespace AlatrafClinic.Application.CommandHandlers;

// public sealed class CreatePaymentHandler : IRequestHandler<CreatePaymentCommand, SaleSagaResult>
// {
//     private readonly SaleSagaOrchestrator _orchestrator;

//     public CreatePaymentHandler(SaleSagaOrchestrator orchestrator)
//     {
//         _orchestrator = orchestrator;
//     }

//     public Task<SaleSagaResult> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
//         => _orchestrator.CreatePaymentAsync(request, cancellationToken);
// }
