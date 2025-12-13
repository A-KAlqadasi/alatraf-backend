
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.TherapyCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;

public class CreateTherapySessionCommandHandler : IRequestHandler<CreateTherapySessionCommand, Result<SessionDto>>
{
    private readonly ILogger<CreateTherapySessionCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IAppDbContext _context;

    public CreateTherapySessionCommandHandler(ILogger<CreateTherapySessionCommandHandler> logger, HybridCache cache, IAppDbContext context)
    {
        _logger = logger;
        _cache = cache;
        _context = context;
    }
    public async Task<Result<SessionDto>> Handle(CreateTherapySessionCommand command, CancellationToken ct)
    {
        var therapyCard = await _context.TherapyCards
        .Include(x => x.DiagnosisPrograms)
            .ThenInclude(x=> x.MedicalProgram)
        .Include(x => x.Sessions)
            .ThenInclude(x=> x.SessionPrograms)
        .FirstOrDefaultAsync(x=> x.Id == command.TherapyCardId, ct);

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
            var doctorSectionRoom = await _context.DoctorSectionRooms
                .Include(x => x.Doctor)
                    .ThenInclude(x=> x.Person)
                .Include(x => x.Section)
                .Include(x => x.Room)
                .FirstOrDefaultAsync(x => x.DoctorId == sessionProgram.DocotorId 
                                          && x.SectionId == sessionProgram.SectionId 
                                          && x.RoomId == sessionProgram.RoomId, ct);
            
            if (doctorSectionRoom is null)
            {
                _logger.LogError("DoctorSectionRoom with DoctorId {DoctorId}, SectionId {SectionId}, RoomId {RoomId} not found or inactive", sessionProgram.DocotorId, sessionProgram.SectionId, sessionProgram.RoomId);

                return DoctorSectionRoomErrors.DoctorSectionRoomNotFound;
            }
            if(!doctorSectionRoom.IsActive)
            {
                _logger.LogError("DoctorSectionRoom with DoctorId {DoctorId}, SectionId {SectionId}, RoomId {RoomId} is inactive", sessionProgram.DocotorId, sessionProgram.SectionId, sessionProgram.RoomId);

                return DoctorSectionRoomErrors.DoctorIsNotActive;
            }

            sessionProgramsData.Add((sessionProgram.DiagnosisProgramId, doctorSectionRoom.Id)); 
        }

        var session = therapyCard.AddSession(sessionProgramsData);
        
        if (session.IsError)
        {
            _logger.LogError("Failed to add session to TherapyCard with id {TherapyCardId}. Error: {Error}", command.TherapyCardId, string.Join(", ", session.Errors));

            return session.TopError;
        }

        _context.TherapyCards.Update(therapyCard);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("session", ct);


        _logger.LogInformation("TherapyCard {TherapyCardId} updated with new session {Number}.", therapyCard.Id, session.Value.Number);
        var s = session.Value;
        
        return s.ToDto();
    }
}