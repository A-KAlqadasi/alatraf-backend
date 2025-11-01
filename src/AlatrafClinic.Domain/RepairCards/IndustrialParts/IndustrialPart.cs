using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.DiagnosisIndustrialParts;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public class IndustrialPart : AuditableEntity<int>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; set; }

    private readonly List<IndustrialPartUnit> _industrialPartUnits = new();
    public IReadOnlyCollection<IndustrialPartUnit> IndustrialPartUnits => _industrialPartUnits.AsReadOnly();
    
    public ICollection<DiagnosisIndustrialPart> DiagnosisIndustrialParts { get; set; } = new List<DiagnosisIndustrialPart>();

    private IndustrialPart() { }

    private IndustrialPart(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public static Result<IndustrialPart> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return IndustrialPartErrors.NameIsRequired;
        }

        return new IndustrialPart(name, description);
    }

    public Result<Updated> Update(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return IndustrialPartErrors.NameIsRequired;
        }

        Name = name;
        Description = description;
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
    public Result<Updated> UpsertUnits(List<IndustrialPartUnit> incomingUnits)
    {
        _industrialPartUnits.RemoveAll(existing => incomingUnits.All(u => u.Id != existing.Id));

        foreach (var incoming in incomingUnits)
        {
            var existing = _industrialPartUnits.FirstOrDefault(u => u.Id == incoming.Id);
            if (existing is null)
            {
                _industrialPartUnits.Add(incoming);
            }
            else
            {
                var updateUnitResult = existing.Update(Id, incoming.UnitId, incoming.PricePerUnit);

                if (updateUnitResult.IsError)
                {
                    return updateUnitResult.Errors;
                }
            }
        }

        return Result.Updated;
    }
}