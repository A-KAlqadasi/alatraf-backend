using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.DiagnosisPrograms;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.Sessions;

namespace AlatrafClinic.Domain.TherapyCards;

public class TherapyCard : AuditableEntity<int>
{
    public DateTime ProgramStartDate { get; set; }
    public DateTime ProgramEndDate { get; set; }
    public int NumberOfSessions { get; set; }
    public bool IsActive { get; set; }
    public int DiagnosisId { get; set; }
    public Diagnosis? Diagnosis { get; set; }
    public TherapyCardType Type { get; set; }
    public decimal SessionPricePerType { get; set; }
    public decimal? TotalPrice => NumberOfSessions * SessionPricePerType;
    private readonly List<TherapyCardStatus> _cardStatuses = new();
    public IReadOnlyCollection<TherapyCardStatus> CardStatuses => _cardStatuses.AsReadOnly();
    public bool IsPaid => _cardStatuses.Any(s => s.TherapyCardId == Id && s.PaymentId is not null);
    public bool IsExpired => DateTime.Now > ProgramEndDate;
    private readonly List<Session> _sessions = new();
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();
    private readonly List<DiagnosisProgram> _diagnosisPrograms = new();
    public IReadOnlyCollection<DiagnosisProgram> DiagnosisPrograms => _diagnosisPrograms.AsReadOnly();
    public int? ParentCardId { get; private set; }
    public TherapyCard? ParentCard { get; private set; }
    public ICollection<TherapyCard> RenewalCards { get; private set; } = new List<TherapyCard>();
    private TherapyCard()
    {

    }
    private TherapyCard(DateTime programStartDate, DateTime programEndDate, TherapyCardType type, decimal sessionPricePerType, int numberOfSessions, List<DiagnosisProgram> diagnosisPrograms, CardStatus status = CardStatus.New, int? parentCardId = null)
    {
        ProgramStartDate = programStartDate;
        ProgramEndDate = programEndDate;
        Type = type;
        SessionPricePerType = sessionPricePerType;
        IsActive = true;
        _diagnosisPrograms = diagnosisPrograms;
        NumberOfSessions = numberOfSessions;
        var cardStatus = TherapyCardStatus.Create(status);
        if (cardStatus.IsError)
        {
            throw new ArgumentException(cardStatus.TopError.Code);
        }
        _cardStatuses.Add(cardStatus.Value);

        ParentCardId = parentCardId;
    }

    public static Result<TherapyCard> Create(DateTime programStartDate, DateTime programEndDate, TherapyCardType type, decimal sessionPricePerType, List<DiagnosisProgram> diagnosisPrograms, CardStatus status = CardStatus.New, int? parentCardId = null)
    {
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

        if (sessionPricePerType <= 0)
        {  
            return TherapyCardErrors.SessionPricePerTypeInvalid;
        }
        
        return new TherapyCard(programStartDate, programEndDate, type, sessionPricePerType, numberOfSessions, diagnosisPrograms, status, parentCardId);
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
    public Result<Updated> AddSession(List<SessionProgram> sessionPrograms)
    {
        var sessionValidate = SessionValidation();
        if (sessionValidate.IsError)
        {
            return sessionValidate.TopError;
        }

        var session = Session.Create(Id, _sessions.Count + 1, sessionPrograms);

        if (session.IsError)
        {
            return session.TopError;
        }

        _sessions.Add(session.Value);
        return Result.Updated;
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
        int lastSessionNumber = lastSession != null ? lastSession.Number ?? 0 : 0;

        var numOfSessions = ProgramEndDate.Subtract(lastSessionDate).Days + 1;

        for (int i = 0; i < numOfSessions; i++)
        {
            var sessionDate = lastSessionDate.AddDays(i);
            var session = Session.Create(lastSessionNumber + i + 1, sessionDate, Id);

            if (session.IsError)
            {
                return session.TopError;
            }

            _sessions.Add(session.Value);
        }

        return Result.Updated;
    }
    
    public Result<Updated> Renew(DateTime newProgramStartDate, DateTime newProgramEndDate, TherapyCardType type, decimal sessionPricePerType, List<DiagnosisProgram> diagnosisPrograms)
    {
        if (!IsExpired)
        {
            return TherapyCardStatusErrors.CardNotExpiredToRenew;
        }
        
        var renewedCardResult = Create(newProgramStartDate, newProgramEndDate, type, sessionPricePerType, diagnosisPrograms, CardStatus.Renew, this.Id);

        if (renewedCardResult.IsError)
        {
            return renewedCardResult.TopError;
        }
        renewedCardResult.Value.DiagnosisId = DiagnosisId;

        RenewalCards.Add(renewedCardResult.Value);
        IsActive = false;

        return Result.Updated;
    }
    public TherapyCard? GetLatestRenewedCard()
    {
        return RenewalCards
            .OrderByDescending(card => card.ProgramStartDate)
            .FirstOrDefault();
    }
    public Result<Updated> ReplacementOfLost()
    {
        if (!IsActive)
        {
            return TherapyCardErrors.Readonly;
        }

        if (IsExpired)
        {
            return TherapyCardStatusErrors.CardExpiredToReplace;
        }

        var card = TherapyCardStatus.Create(CardStatus.ReplacementOfLost);
        if (card.IsError)
        {
            return card.TopError;
        }

        _cardStatuses.Add(card.Value);

        return Result.Updated;
    }
}