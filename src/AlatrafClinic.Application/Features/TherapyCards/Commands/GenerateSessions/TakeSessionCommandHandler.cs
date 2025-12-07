using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
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

        List<(int diagnosisProgramId, int doctorSectionRoomId)> sessionProgramsData = new();
        foreach (var sessionProgram in command.SessionProgramsData)
        {
            var doctor = await _unitOfWork.DoctorSectionRooms.GetByIdAsync(sessionProgram.DoctorSectionRoomId, ct);
            if(doctor is null)
            {
                _logger.LogError("Doctor section room with Id {sectionRoomId} is not found", sessionProgram.DoctorSectionRoomId);
                return DoctorSectionRoomErrors.DoctorSectionRoomNotFound;
            }
            if (!doctor.IsActive)
            {
                _logger.LogError("Doctor with section room id {sectionRoomId} is not active", sessionProgram.DoctorSectionRoomId);
                return DoctorSectionRoomErrors.DoctorIsNotActive;
            }

            sessionProgramsData.Add((sessionProgram.DiagnosisProgramId, sessionProgram.DoctorSectionRoomId)); 
        }

        var result = therapyCard.TakeSession(command.SessionId, sessionProgramsData);
        if (result.IsError)
        {
            _logger.LogError("Failed to take session for therapy card with id {TherapyCardId}.", command.TherapyCardId);

            return result.Errors;
        }
        await _unitOfWork.TherapyCards.UpdateAsync(therapyCard, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Session with id {SessionId} taken for therapy card with id {TherapyCardId}.", command.SessionId, command.TherapyCardId);
        
        //var session = therapyCard.Sessions.FirstOrDefault(s=> s.Id == command.SessionId);
        return Result.Success;
    }
}