using System.Collections;

using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Domain.TherapyCards;

public class TherapyCard : AuditableEntity<int>
{
    public DateTime ProgramStartDate { get; private set; }
    public DateTime ProgramEndDate { get; private set; }
    public int NumberOfSessions { get; private set; }
    public bool IsActive { get; private set; }
    public int DiagnosisId { get; private set; }
    public Diagnosis Diagnosis { get; set; } = default!;
    public bool IsPaid => Diagnosis.Payments.Any(p => p.DiagnosisId == DiagnosisId && p.IsCompleted);
    public Payment? Payment => Diagnosis.Payments.FirstOrDefault(p => p.DiagnosisId == DiagnosisId);
    
    public TherapyCardType Type { get; private set; }
    public string? Notes { get; private set; }
    public decimal SessionPricePerType { get; private set; }
    public decimal TotalCost => NumberOfSessions * SessionPricePerType;
    public bool IsExpired => DateTime.Now.Date > ProgramEndDate.Date;
    public bool IsEditable => IsActive && !IsExpired && !IsPaid && _sessions.Count() == 0;
    private readonly List<Session> _sessions = new();
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();
    public Session? LatestSession => _sessions.OrderByDescending(s => s.SessionDate).FirstOrDefault();
     
    private readonly List<DiagnosisProgram> _diagnosisPrograms = new();
    public IReadOnlyCollection<DiagnosisProgram> DiagnosisPrograms => _diagnosisPrograms.AsReadOnly();
    public int? ParentCardId { get; private set; }
    public TherapyCard? ParentCard { get; private set; }
    public TherapyCardStatus CardStatus { get; private set; } 
    private TherapyCard()
    {

    }
    private TherapyCard(int diagnosisId, DateTime programStartDate, DateTime programEndDate, TherapyCardType type, decimal sessionPricePerType, int numberOfSessions, List<DiagnosisProgram> diagnosisPrograms, TherapyCardStatus status = TherapyCardStatus.New, int? parentCardId = null, string? notes = null)
    {
        DiagnosisId = diagnosisId;
        ProgramStartDate = programStartDate;
        ProgramEndDate = programEndDate;
        Type = type;
        SessionPricePerType = sessionPricePerType;
        IsActive = true;
        _diagnosisPrograms = diagnosisPrograms;
        NumberOfSessions = numberOfSessions;
        Notes = notes;
        this.CardStatus = status;
        ParentCardId = parentCardId;
    }

    public static Result<TherapyCard> Create(int diagnosisId, DateTime programStartDate, DateTime programEndDate, TherapyCardType type, decimal sessionPricePerType, List<DiagnosisProgram> diagnosisPrograms, TherapyCardStatus status, int? parentCardId = null, string? notes = null)
    {
        if (diagnosisId <= 0)
        {
            return TherapyCardErrors.InvalidDiagnosisId;
        }

        if (programStartDate < DateTime.Now)
        {
            return TherapyCardErrors.ProgramStartDateNotInPast;
        }

        if (programEndDate <= programStartDate)
        {
            return TherapyCardErrors.InvalidTiming;
        }

        int numberOfSessions = (programEndDate - programStartDate).Days + 1;

        if (!Enum.IsDefined(typeof(TherapyCardType), type))
        {
            return TherapyCardErrors.TherapyCardTypeInvalid;
        }
        if(!Enum.IsDefined(typeof(TherapyCardStatus), status))
        {
            return TherapyCardErrors.InvalidCardStatus;
        }

        if (sessionPricePerType <= 0)
        {
            return TherapyCardErrors.SessionPricePerTypeInvalid;
        }

        return new TherapyCard(diagnosisId, programStartDate, programEndDate, type, sessionPricePerType, numberOfSessions, diagnosisPrograms, status, parentCardId, notes);
    }

    public Result<Updated> Update(DateTime programStartDate, DateTime programEndDate, TherapyCardType type, decimal sessionPricePerType, string? notes = null)
    {
        if (!IsEditable)
        {
            return TherapyCardErrors.Readonly;
        }

        if (programStartDate < DateTime.Now)
        {
            return TherapyCardErrors.ProgramStartDateNotInPast;
        }

        if (programEndDate <= programStartDate)
        {
            return TherapyCardErrors.InvalidTiming;
        }

        if (!Enum.IsDefined(typeof(TherapyCardType), type))
        {
            return TherapyCardErrors.TherapyCardTypeInvalid;
        }

        if (sessionPricePerType <= 0)
        {
            return TherapyCardErrors.SessionPricePerTypeInvalid;
        }

        ProgramStartDate = programStartDate;
        ProgramEndDate = programEndDate;
        Type = type;
        SessionPricePerType = sessionPricePerType;
        Notes = notes;
        NumberOfSessions = (ProgramEndDate - ProgramStartDate).Days + 1;

        return Result.Updated;
    }
    
    public Result<Updated> UpsertDiagnosisPrograms(List<DiagnosisProgram> diagnosisPrograms)
    {
        if (!IsEditable)
        {
            return TherapyCardErrors.Readonly;
        }

        _diagnosisPrograms.Clear();
        _diagnosisPrograms.AddRange(diagnosisPrograms);

        return Result.Updated;
    }

    public Result<Updated> DeActivate()
    {
        if (!IsActive)
        {
            return TherapyCardErrors.Readonly;
        }

        IsActive = false;
        return Result.Updated;
    }
    private Result<Success> SessionValidation()
    {
        if (!IsActive)
        {
            return TherapyCardErrors.Readonly;
        }

        if (!IsPaid)
        {
            return TherapyCardErrors.IsNotPaid;
        }

        if (DateTime.Now > ProgramEndDate)
        {
            return TherapyCardErrors.ProgramEnded;
        }

        if (_sessions.Count >= NumberOfSessions)
        {
            return TherapyCardErrors.ProgramEnded;
        }
        ;
        return Result.Success;
    }
    public Result<Session> AddSession(List<(int diagnosisProgramId, int doctorSectionRoomId)> sessionProgramsData)
    {
        var sessionValidate = SessionValidation();
        if (sessionValidate.IsError)
        {
            return sessionValidate.Errors;
        }

        var session = Session.Create(Id, _sessions.Count + 1);

        if (session.IsError)
        {
            return session.TopError;
        }
        var assignProgramsResult = session.Value.TakeSession(sessionProgramsData);

        if (assignProgramsResult.IsError)
        {
            return assignProgramsResult.TopError;
        }

        _sessions.Add(session.Value);

        return session.Value;
    }
    public Result<Updated> TakeSession(int sessionId, List<(int diagnosisProgramId, int doctorSectionRoomId)> sessionProgramsData)
    {
        var session = _sessions.FirstOrDefault(s => s.Id == sessionId);
        if (session == null)
        {
            return TherapyCardErrors.SessionNotFound;
        }
        

        return session.TakeSession(sessionProgramsData);
    }

    public Result<Updated> GenerateSessions()
    {
        var sessionValidate = SessionValidation();
        if (sessionValidate.IsError)
        {
            return sessionValidate.TopError;
        }

        var lastSession = _sessions.OrderByDescending(s => s.SessionDate).FirstOrDefault();

        DateTime lastSessionDate = lastSession != null ? lastSession.SessionDate.AddDays(1) : ProgramStartDate;
        int lastSessionNumber = lastSession != null ? lastSession.Number : 0;

        var numOfSessions = ProgramEndDate.Subtract(lastSessionDate).Days + 1;

        for (int i = 0; i < numOfSessions; i++)
        {
            var sessionDate = lastSessionDate.AddDays(i);
            var session = Session.Create(Id, lastSessionNumber + i + 1, sessionDate);

            if (session.IsError)
            {
                return session.TopError;
            }

            _sessions.Add(session.Value);
        }

        return Result.Updated;
    }
}