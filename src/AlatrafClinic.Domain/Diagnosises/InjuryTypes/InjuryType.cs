using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.InjuryTypes;

public class InjuryType : AuditableEntity<int>
{
    public string? Name { get; set; }

    public ICollection<Diagnosis> Diagnosises { get; set; } = new List<Diagnosis>();
    private InjuryType()
    {
    }
    private InjuryType(string? name)
    {
        Name = name;
    }

    public static Result<InjuryType> Create(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return InjuryTypeErrors.NameIsRequired;
        }

        return new InjuryType(name);
    }

    public Result<Updated> Update(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return InjuryTypeErrors.NameIsRequired;
        }
        
        Name = name;
        return Result.Updated;
    }
}