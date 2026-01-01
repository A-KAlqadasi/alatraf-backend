using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.DoctorSectionRooms;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.AssignDoctorToSectionAndRoom;

public class AssignDoctorToSectionAndRoomCommandHandler(
    IAppDbContext _context,
    ILogger<AssignDoctorToSectionAndRoomCommandHandler> _logger
) : IRequestHandler<AssignDoctorToSectionAndRoomCommand, Result<Updated>>
{
    
    public async Task<Result<Updated>> Handle(AssignDoctorToSectionAndRoomCommand command, CancellationToken ct)
    {
        var doctor = await _context.Doctors
        .Include(d=> d.Assignments)
        
        .FirstOrDefaultAsync(d=> d.Id == command.DoctorId, ct);
        if (doctor is null)
        {
            _logger.LogWarning("Doctor {DoctorId} not found.", command.DoctorId);
            return ApplicationErrors.DoctorNotFound;
        }

        var section = await _context.Sections.FirstOrDefaultAsync(s=> s.Id == command.SectionId, ct);
        if (section is null)
        {
            _logger.LogWarning("Section {SectionId} not found.", command.SectionId);
            return ApplicationErrors.SectionNotFound;
        }
        
        Room? room = null;

        if (command.RoomId.HasValue)
        {
            room = await _context.Rooms.FirstOrDefaultAsync(r=> r.Id == command.RoomId, ct);
            if (room is null)
            {
                _logger.LogWarning("Room {RoomId} not found.", command.RoomId);
                return ApplicationErrors.RoomNotFound;
            }
        }
        Result<DoctorSectionRoom> assignResult;
        
        if(room != null)
        {
            assignResult = doctor.AssignToSectionAndRoom(section, room, command.Notes);
        }
        else
        {
            assignResult = doctor.AssignToSection(section, command.Notes);
        }

        if (assignResult.IsError)
        {
            _logger.LogWarning("Failed to assign Doctor {DoctorId} to Section {SectionId} / Room {RoomId}: {Error}",
                doctor.Id, section.Id, room?.Id, assignResult.Errors);
            return assignResult.Errors;
        }

        if (command.IsActive)
        {
            doctor.Activate();
        }
        else
        {
            doctor.DeActivate();
        }

        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Doctor {DoctorId} assigned to Section {SectionId} / Room {RoomId}.",
            doctor.Id, section.Id, room?.Id);

        return Result.Updated;
    }
}