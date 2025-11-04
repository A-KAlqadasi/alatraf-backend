using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.Sessions;

public class Session : AuditableEntity<int>
{
    public bool IsTaken { get; private set; } = false;
    public int Number { get; private set; }
    public int TherapyCardId { get; private set; }
    public TherapyCard? TherapyCard { get; private set; }
    public DateTime SessionDate { get; private set; }

    private readonly List<SessionProgram> _sessionPrograms = new();
    public IEnumerable<SessionProgram> SessionPrograms => _sessionPrograms.AsReadOnly();
    private Session()
    {
    }

    private Session(int therapyCardId, int number)
    {
        TherapyCardId = therapyCardId;
        Number = number;
        SessionDate = DateTime.Now;
    }
    private Session(int therapyCardId, int number, DateTime date)
    {
        TherapyCardId = therapyCardId;
        Number = number;
        SessionDate = date;
    }
    public static Result<Session> Create(int therapyCardId, int number, DateTime date)
    {
        if (therapyCardId <= 0)
        {
            return SessionErrors.TherapyCardIdIsRequired;
        }
        if (number <= 0)
        {
            return SessionErrors.NumberIsRequired;
        }

        return new Session(therapyCardId, number, date);
    }

    public static Result<Session> Create(int therapyCardId, int number)
    {
        if (therapyCardId <= 0)
        {
            return SessionErrors.TherapyCardIdIsRequired;
        }

        if (number <= 0)
        {
            return SessionErrors.NumberIsRequired;
        }
        return new Session(therapyCardId, number);
    }
    
    public Result<Updated> TakeSession(List<(int diagnosisProgramId, int doctorSectionRoomId)> sessionProgramsData)
    {
        if (IsTaken)
        {
            return SessionErrors.SessionAlreadyTaken;
        }
        if (SessionDate.Date != DateTime.Now.Date)
        {
            return SessionErrors.InvalidSessionDate(SessionDate);
        }
        if (sessionProgramsData == null || !sessionProgramsData.Any())
        {
            return SessionErrors.SessionProgramsAreRequired;
        }

        foreach (var (diagnosisProgramId, doctorSectionRoomId) in sessionProgramsData)
        {
            var sessionProgramResult = SessionProgram.Create(diagnosisProgramId, doctorSectionRoomId);
            if (sessionProgramResult.IsError)
            {
                return sessionProgramResult.TopError;
            }
            _sessionPrograms.Add(sessionProgramResult.Value);
        }
        IsTaken = true;
        return Result.Updated;
    }
}