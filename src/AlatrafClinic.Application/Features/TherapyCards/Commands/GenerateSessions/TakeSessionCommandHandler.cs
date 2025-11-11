using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;

public class TakeSessionCommandHandler : IRequestHandler<TakeSessionCommand, Result<Success>>
{
    private readonly ILogger<TakeSessionCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public TakeSessionCommandHandler(ILogger<TakeSessionCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Success>> Handle(TakeSessionCommand command, CancellationToken ct)
    {
        var therapyCard = await _unitOfWork.TherapyCards.GetByIdAsync(command.TherapyCardId, ct);
        if (therapyCard is null)
        {
            _logger.LogError("Therapy card with id {TherapyCardId} not found.", command.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }
        var result = therapyCard.TakeSession(command.SessionId, command.SessionProgramsData);
        if (result.IsError)
        {
            _logger.LogError("Failed to take session for therapy card with id {TherapyCardId}.", command.TherapyCardId);

            return result.Errors;
        }

        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Session with id {SessionId} taken for therapy card with id {TherapyCardId}.", command.SessionId, command.TherapyCardId);
        
        return Result.Success;
    }
}