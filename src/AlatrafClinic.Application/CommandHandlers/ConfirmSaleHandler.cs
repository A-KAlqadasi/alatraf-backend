using System.Threading;
using System.Threading.Tasks;

using AlatrafClinic.Application.Commands;
using AlatrafClinic.Application.Sagas;

using MediatR;

namespace AlatrafClinic.Application.CommandHandlers;

public sealed class ConfirmSaleHandler : IRequestHandler<ConfirmSaleCommand, SaleSagaResult>
{
    private readonly SaleSagaOrchestrator _orchestrator;

    public ConfirmSaleHandler(SaleSagaOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    public Task<SaleSagaResult> Handle(ConfirmSaleCommand request, CancellationToken cancellationToken)
        => _orchestrator.ConfirmSaleAsync(request, cancellationToken);
}
