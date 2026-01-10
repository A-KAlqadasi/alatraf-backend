using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.TherapyCards;
using AlatrafClinic.Domain.TherapyCards.Enums;
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
            NumberOfSessions = entity.NumberOfTakenSessions,
            ProgramStartDate = entity.ProgramStartDate,
            ProgramEndDate = entity.ProgramEndDate,
            TherapyCardType = entity.Type.ToArabicTherapyCardType(),
            CardStatus = entity.CardStatus.ToArabicTherapyCardStatus(),
            Notes = entity.Notes,
            Programs = entity.Diagnosis?.DiagnosisPrograms?.ToDtos(),
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
        var sectionName = program.DoctorSectionRoom?.Section?.Name ?? string.Empty;
        var roomName = program.DoctorSectionRoom?.Room?.Name ?? string.Empty;
        string roomSectionName = ($"{sectionName} - {roomName}").Trim(' ', '-');
        
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

    public static TherapyCardDiagnosisDto ToTherapyDiagnosisDto(this TherapyCard entity)
    {
        return new TherapyCardDiagnosisDto
        {
            TicketId = entity.Diagnosis.TicketId,

            PatientId = entity.Diagnosis.PatientId,

            PatientName = entity.Diagnosis.Patient.Person.FullName,

            Gender  = entity.Diagnosis.Patient.Person.Gender ? "ذكر" : "أنثى",
            Age =entity.Diagnosis.Patient.Person.Age,
            DiagnosisId = entity.DiagnosisId,

            DiagnosisText = entity.Diagnosis.DiagnosisText,

            InjuryDate  = entity.Diagnosis.InjuryDate,

            DiagnosisType = entity.Diagnosis.DiagnoType.ToArabicDiagnosisType(),

            InjuryReasons = entity.Diagnosis.InjuryReasons.ToDtos(),
            InjurySides = entity.Diagnosis.InjurySides.ToDtos(),
            InjuryTypes = entity.Diagnosis.InjuryTypes.ToDtos(),
            Programs = entity.Diagnosis.DiagnosisPrograms.ToDtos(),
            TherapyCardId  = entity.Id,
            ProgramStartDate = entity.ProgramStartDate,
            ProgramEndDate  = entity.ProgramEndDate,

            TherapyCardType = entity.Type.ToArabicTherapyCardType(),

            CardStatus = entity.CardStatus.ToArabicTherapyCardStatus(),

            Notes = entity.Notes
        
        };
    }

    public static List<TherapyCardDiagnosisDto> ToTherapyDiagnosisDtos(this IEnumerable<TherapyCard> therapyCards)
    {
        return therapyCards.Select(t=> t.ToTherapyDiagnosisDto()).ToList();
    }
    

    public static string ToArabicTherapyCardType(this TherapyCardType type)
    {
        switch (type)
        {
            case TherapyCardType.General : return "عام";
            case TherapyCardType.Special : return "خاص";
            case TherapyCardType.NerveKids : return "أطفال اعصاب";
            default:
             throw new Exception("Therapy card type is unknown");
        }
    }

    public static string ToArabicTherapyCardStatus(this TherapyCardStatus status)
    {
        switch (status)
        {
            case TherapyCardStatus.New : return "لأول مرة";
            case TherapyCardStatus.Renew : return "مجدد";
            default:
            throw new Exception("Therapy card status is unknown");
        }
    }
}