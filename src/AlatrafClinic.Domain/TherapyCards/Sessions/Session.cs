using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.Sessions;

public class Session : AuditableEntity<int>
{
    public bool IsTaken { get; private set; }
    public int Number { get; private set; }
    public int TherapyCardId { get; private set; }
    public TherapyCard? TherapyCard { get; private set; }
    public DateTime SessionDate { get; private set; }

    private readonly List<SessionProgram> _sessionPrograms = new();
    public IEnumerable<SessionProgram> SessionPrograms => _sessionPrograms.AsReadOnly();
    private Session()
    {
    }

    private Session(int therapyCardId, int number, List<SessionProgram> sessionPrograms, bool isTaken)
    {
        TherapyCardId = therapyCardId;
        Number = number;
        IsTaken = isTaken;
        _sessionPrograms = sessionPrograms;
        SessionDate = DateTime.Now;
    }
    private Session(int number, DateTime date, int therapyCardId)
    {
        Number = number;
        SessionDate = date;
        TherapyCardId = therapyCardId;
        IsTaken = false;
    }
    public static Result<Session> Create(int number, DateTime date, int therapyCardId)
    {
        if (therapyCardId <= 0)
        {
            return SessionErrors.TherapyCardIdIsRequired;
        }
        if (number <= 0)
        {
            return SessionErrors.NumberIsRequired;
        }

        return new Session(number, date, therapyCardId);
    }

    public static Result<Session> Create(int therapyCardId, int number, List<SessionProgram> sessionPrograms, bool isTaken = true)
    {
        if (therapyCardId <= 0)
        {
            return SessionErrors.TherapyCardIdIsRequired;
        }
        if (number <= 0)
        {
            return SessionErrors.NumberIsRequired;
        }
        return new Session(therapyCardId, number, sessionPrograms, isTaken);
    }
    public Result<Updated> TakeSession(List<SessionProgram> sessionPrograms)
    {
        if (IsTaken == true)
        {
            return SessionErrors.SessionAlreadyTaken;
        }
        
        if(SessionDate.Date != DateTime.Now.Date)
        {
            return SessionErrors.InvalidSessionDate(SessionDate);
        }
        _sessionPrograms.AddRange(sessionPrograms);
        IsTaken = true;

        return Result.Updated;
    }

}