using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Organization.DoctorSectionRooms;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Application.Features.TherapyCards.Mappers;

public static class TherapyCardMapper
{
    public static TherapyCardDto ToDto(this TherapyCard entity)
    {
        if (entity is null) return new TherapyCardDto();

        return new TherapyCardDto
        {
            TherapyCardId = entity.Id,
            Diagnosis = entity.Diagnosis != null ? entity.Diagnosis.ToDto() : new DiagnosisDto(),
            IsActive = entity.IsActive,
            NumberOfSessions = entity.NumberOfSessions,
            ProgramStartDate = entity.ProgramStartDate,
            ProgramEndDate = entity.ProgramEndDate,
            TherapyCardType = entity.Type,
            CardStatus = entity.CardStatus,
            Notes = entity.Notes,
            Programs = entity.DiagnosisPrograms?.ToDtos(),
            Sessions = entity.Sessions?.ToDtos()
        };
    }

    public static List<TherapyCardDto> ToDtos(this IEnumerable<TherapyCard> therapyCards)
    {
        return therapyCards.Select(t => t.ToDto()).ToList();
    }

    // ------------------ Sessions ------------------

    public static List<SessionDto> ToDtos(this IEnumerable<Session> sessions)
    {
        return sessions.Select(s => s.ToDto()).ToList();
    }

    public static SessionDto ToDto(this Session session)
    {
        if (session is null) return new SessionDto();

        return new SessionDto
        {
            SessionId = session.Id,
            IsTaken = session.IsTaken,
            Number = session.Number,
            SessionDate = session.SessionDate,
            SessionPrograms = session.SessionPrograms?.ToDtos() ?? new List<SessionProgramDto>()
        };
    }

    // ------------------ SessionPrograms ------------------

    public static List<SessionProgramDto> ToDtos(this IEnumerable<SessionProgram> programs)
    {
        return programs.Select(p => p.ToDto()).ToList();
    }

    public static SessionProgramDto ToDto(this SessionProgram program)
    {
        if (program is null) return new SessionProgramDto();
        string roomSectionName = program.DoctorSectionRoom?.Section.Name + " - " + program.DoctorSectionRoom?.Room?.Name.ToString();
        roomSectionName = roomSectionName.Trim(new char[] { ' ', '-' });
        return new SessionProgramDto
        {
            SessionProgramId = program.Id,
            DiagnosisProgramId = program.DiagnosisProgramId,
            ProgramName = program.DiagnosisProgram?.MedicalProgram?.Name ?? string.Empty,
            DoctorSectionRoomId = program.DoctorSectionRoomId,
            DoctorSectionRoomName = roomSectionName,
            DoctorName = program.DoctorSectionRoom?.Doctor?.Person?.FullName
        };
    }

   
}