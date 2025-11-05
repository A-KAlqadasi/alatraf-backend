using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.Diagnosises.InjurySides;

public class InjurySide : AuditableEntity<int>
{
    public string Name { get; private set; } = string.Empty;

    private InjurySide()
    {
    }
    private InjurySide(string name)
    {
        Name = name;
    }

    public static Result<InjurySide> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return InjurySideErrors.NameIsRequired;
        }

        return new InjurySide(name);
    }

    public Result<Updated> Update(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return InjurySideErrors.NameIsRequired;
        }
        
        Name = name;
        return Result.Updated;
    }
}