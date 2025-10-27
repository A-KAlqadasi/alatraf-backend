using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public class IndustrialPart : AuditableEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    private readonly List<IndustrialPartUnit> _industrialPartUnits = new();
    public IReadOnlyCollection<IndustrialPartUnit> IndustrialPartUnits => _industrialPartUnits.AsReadOnly();
    
    public ICollection<DiagnosisIndustrialPart> DiagnosisIndustrialParts { get; set; } = new List<DiagnosisIndustrialPart>();

    private IndustrialPart() { }

    private IndustrialPart(string? name, string? description, List<IndustrialPartUnit> industrialPartUnits)
    {
        Name = name;
        Description = description;
        _industrialPartUnits = industrialPartUnits;
    }

    public static Result<IndustrialPart> Create(string? name, string? description, List<IndustrialPartUnit> industrialPartUnits)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return IndustrialPartErrors.NameIsRequired;
        }

        return new IndustrialPart(name, description, industrialPartUnits);
    }

    public Result<Updated> Update(string? name, string? description, List<IndustrialPartUnit> industrialPartUnits)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return IndustrialPartErrors.NameIsRequired;
        }

        Name = name;
        Description = description;
        _industrialPartUnits.Clear();
        _industrialPartUnits.AddRange(industrialPartUnits);

        return Result.Updated;
    }
    public Result<Updated> AssignUnit(IndustrialPartUnit industrialPartUnit)
    {
        var doesUnitExists = _industrialPartUnits.Any(u => u.Unit?.Name == industrialPartUnit?.Unit?.Name);
        if (doesUnitExists)
        {
            return IndustrialPartErrors.UnitAlreadyExists;
        }
        
        _industrialPartUnits.Add(industrialPartUnit);
        return Result.Updated;
    }
}