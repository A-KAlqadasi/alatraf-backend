using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.TherapyCards.Sessions;

public class Session : AuditableEntity<int>
{
    public bool? IsTaken { get; set; }
    public int? Number { get; set; }
    public int? TherapyCardId { get; set; }
    public TherapyCard? TherapyCard { get; set; }

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

}