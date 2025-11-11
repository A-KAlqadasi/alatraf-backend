
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;

public class GenerateSessionsCommandHandler : IRequestHandler<GenerateSessionsCommand, Result<List<SessionDto>>>
{
    private readonly ILogger<GenerateSessionsCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GenerateSessionsCommandHandler(ILogger<GenerateSessionsCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<List<SessionDto>>> Handle(GenerateSessionsCommand command, CancellationToken ct)
    {
        var therapyCard = await _unitOfWork.TherapyCards.GetByIdAsync(command.TherapyCardId, ct);
        if (therapyCard is null)
        {
            _logger.LogError("Therapy card with ID {TherapyCardId} not found.", command.TherapyCardId);

            return TherapyCardErrors.TherapyCardNotFound;
        }
        var result = therapyCard.GenerateSessions();
        if (result.IsError)
        {
            return result.Errors;
        }


        await _unitOfWork.TherapyCards.UpdateAsync(therapyCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var sessions = therapyCard.Sessions.Where(x => DateTime.Now >= x.SessionDate).ToList();

        _logger.LogInformation("Generated {SessionCount} sessions for Therapy Card ID {TherapyCardId}.", sessions.Count, command.TherapyCardId);
        
        return sessions.ToDtos();
    }
}