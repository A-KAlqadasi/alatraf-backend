using System.Threading;
using System.Threading.Tasks;

using AlatrafClinic.Application.Commands;
using AlatrafClinic.Application.Sagas;

using MediatR;

namespace AlatrafClinic.Application.CommandHandlers;

public sealed class StartSaleSagaHandler : IRequestHandler<StartSaleSagaCommand, SaleSagaResult>
{
    private readonly SaleSagaOrchestrator _orchestrator;

    public StartSaleSagaHandler(SaleSagaOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public Task<SaleSagaResult> Handle(StartSaleSagaCommand request, CancellationToken cancellationToken)
        => _orchestrator.StartAsync(request, cancellationToken);
}
