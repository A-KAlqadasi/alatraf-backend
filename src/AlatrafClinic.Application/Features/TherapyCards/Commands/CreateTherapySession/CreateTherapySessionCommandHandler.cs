
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
            _logger.LogError("TherapyCard with id {TherapyCardId} not found", command.TherapyCardId);
            
            return TherapyCardErrors.TherapyCardNotFound;
        }
        if (therapyCard.IsExpired)
        {
            _logger.LogError("TherapyCard with id {TherapyCardId} is expired", command.TherapyCardId);

            return TherapyCardErrors.TherapyCardExpired;
        }
        List<(int diagnosisProgramId, int doctorSectionRoomId)> sessionProgramsData = new();
        foreach (var sessionProgram in command.SessionProgramsData)
        {
            sessionProgramsData.Add((sessionProgram.DiagnosisProgramId, sessionProgram.DoctorSectionRoomId)); 
        }

        var session = therapyCard.AddSession(sessionProgramsData);
        
        if (session.IsError)
        {
            _logger.LogError("Failed to add session to TherapyCard with id {TherapyCardId}. Error: {Error}", command.TherapyCardId, string.Join(", ", session.Errors));

            return session.TopError;
        }

        await _unitOfWork.TherapyCards.UpdateAsync(therapyCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("session", ct);


        _logger.LogInformation("TherapyCard {TherapyCardId} updated with new session {Number}.", therapyCard.Id, session.Value.Number);

        return session.Value.ToDto();
    }
}