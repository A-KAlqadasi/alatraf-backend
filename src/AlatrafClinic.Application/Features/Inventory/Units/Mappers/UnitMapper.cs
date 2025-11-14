using AlatrafClinic.Application.Features.Inventory.Units.Dtos;
using AlatrafClinic.Domain.Inventory.Units;

namespace AlatrafClinic.Application.Features.Inventory.Units.Mappers;

public static class UnitMapper
{
    public static UnitDto ToDto(this Unit unit)
    {
        return new UnitDto
        {
            Id = unit.Id,
            Name = unit.Name!
        };

    }

    public static List<UnitDto> ToDtoList(this IEnumerable<Unit> units)
        => units.Select(ToDto).ToList();

}
