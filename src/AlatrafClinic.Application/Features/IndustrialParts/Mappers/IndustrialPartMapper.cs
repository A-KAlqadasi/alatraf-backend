using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

namespace AlatrafClinic.Application.Features.IndustrialParts.Mappers;

public static class IndustrialPartMapper
{
    public static IndustrialPartDto ToDto(this IndustrialPart industrialPart)
    {
        return new IndustrialPartDto
        {
            IndustrialPartId = industrialPart.Id,
            Name = industrialPart.Name,
            Description = industrialPart.Description,
            IndustrialPartUnits = industrialPart.IndustrialPartUnits.ToDtos()
        };
    }
    public static List<IndustrialPartDto> ToDtos(this IEnumerable<IndustrialPart> industrialParts)
    {
        return industrialParts.Select(ip => ip.ToDto()).ToList();
    }
    public static IndustrialPartUnitDto ToDto(this IndustrialPartUnit industrialPartUnit)
    {
        return new IndustrialPartUnitDto
        {
            IndustrialPartUnitId = industrialPartUnit.Id,
            UnitId = industrialPartUnit.Unit?.Id ?? 0,
            UnitName = industrialPartUnit.Unit?.Name ?? string.Empty,
            PricePerUnit = industrialPartUnit.PricePerUnit
        };
    }
    public static List<IndustrialPartUnitDto> ToDtos(this IEnumerable<IndustrialPartUnit> industrialPartUnits)
    {
        return industrialPartUnits.Select(ipu => ipu.ToDto()).ToList();
    }
}