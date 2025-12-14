using AlatrafClinic.Domain.Common;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Domain.RepairCards.IndustrialParts;

public class IndustrialPart : AuditableEntity<int>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; set; }

    private readonly List<IndustrialPartUnit> _industrialPartUnits = new();
    public IReadOnlyCollection<IndustrialPartUnit> IndustrialPartUnits => _industrialPartUnits.AsReadOnly();
    
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
    public Result<Updated> AssignUnit(List<(int unitId, decimal price)> unit)
    {
        var doesUnitExists = _industrialPartUnits.Any(u => unit.Any(i => i.unitId == u.Unit?.Id));

        if (doesUnitExists)
        {
            return IndustrialPartErrors.UnitAlreadyExists;
        }
        foreach (var (unitId, price) in unit)
        {
            var industrialPartUnit = IndustrialPartUnit.Create(this.Id, unitId, price);

            if (industrialPartUnit.IsError)
            {
                return industrialPartUnit.Errors;
            }

            _industrialPartUnits.Add(industrialPartUnit.Value);
        }

        return Result.Updated;
    }
    public Result<Updated> UpsertUnits(List<(int unitId, decimal price)> incomingUnits)
    {
        _industrialPartUnits.RemoveAll(existing => incomingUnits.All(u => u.unitId != existing.UnitId));

        foreach (var (unitId, price) in incomingUnits)
        {
            var existing = _industrialPartUnits.FirstOrDefault(u => u.UnitId == unitId);
            if (existing is null)
            {
                var createUnitResult = IndustrialPartUnit.Create(this.Id, unitId, price);
                if (createUnitResult.IsError)
                {
                    return createUnitResult.Errors;
                }

                _industrialPartUnits.Add(createUnitResult.Value);
            }
            else
            {
                var updateUnitResult = existing.Update(this.Id, unitId, price);

                if (updateUnitResult.IsError)
                {
                    return updateUnitResult.Errors;
                }
            }
        }

        return Result.Updated;
    }
}