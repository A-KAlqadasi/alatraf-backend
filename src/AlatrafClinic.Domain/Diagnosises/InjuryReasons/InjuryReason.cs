using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.InjuryReasons;

public class InjuryReason : AuditableEntity<int>
{
    public string? Name { get; set; }

    public ICollection<Diagnosis> Diagnosises { get; set; } = new List<Diagnosis>();
    private InjuryReason()
    {
    }
    private InjuryReason(string? name)
    {
        Name = name;
    }

    public static Result<InjuryReason> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return InjuryReasonErrors.NameIsRequired;
        }

        return new InjuryReason(name);
    }

    public Result<Updated> Update(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return InjuryReasonErrors.NameIsRequired;
        }

        Name = name;
        return Result.Updated;
    }
}