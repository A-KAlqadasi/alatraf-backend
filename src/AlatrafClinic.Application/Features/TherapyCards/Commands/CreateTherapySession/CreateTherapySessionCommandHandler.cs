
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public class CreateTherapySessionCommandHandler : IRequestHandler<CreateTherapySessionCommand, Result<SessionDto>>
{
    private readonly ILogger<CreateTherapySessionCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTherapySessionCommandHandler(ILogger<CreateTherapySessionCommandHandler> logger, HybridCache cache, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _cache = cache;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<SessionDto>> Handle(CreateTherapySessionCommand command, CancellationToken ct)
    {
        var therapyCard = await _unitOfWork.TherapyCards.GetByIdAsync(command.TherapyCardId, ct);
        if (therapyCard is null)
        {
            _logger.LogWarning("TherapyCard with id {TherapyCardId} not found", command.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }
        if (therapyCard.IsExpired)
        {
            _logger.LogWarning("TherapyCard with id {TherapyCardId} is expired", command.TherapyCardId);
            return TherapyCardErrors.TherapyCardExpired;
        }
        var session = therapyCard.AddSession(command.SessionProgramsData);
        
        if (session.IsError)
        {
            _logger.LogWarning("Failed to add session to TherapyCard with id {TherapyCardId}. Error: {Error}", command.TherapyCardId, string.Join(", ", session.Errors));

            return session.TopError;
        }

        await _unitOfWork.TherapyCards.UpdateAsync(therapyCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return session.Value.ToDto();
    }
}