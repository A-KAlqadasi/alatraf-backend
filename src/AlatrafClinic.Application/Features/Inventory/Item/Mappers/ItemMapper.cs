using AlatrafClinic.Application.Features.Inventory.Items.Dtos;
using AlatrafClinic.Domain.Inventory.Items;

namespace AlatrafClinic.Application.Features;

public static class ItemMapper
{
    public static ItemDto ToDto(this Item item)
    {
        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            IsActive = item.IsActive,
            BaseUnitId = item.BaseUnitId,
            BaseUnitName = item.BaseUnit.Name,
            Units = item.ItemUnits.Select(u => new ItemUnitDto
            {
                UnitId = u.UnitId,
                Price = u.Price,
                ConversionFactor = u.ConversionFactor,
                MinPriceToPay = u.MinPriceToPay,
                MaxPriceToPay = u.MaxPriceToPay
            }).ToList()
        };
    }

    public static List<ItemDto> ToDtoList(this IEnumerable<Item> items)
          => items.Select(ToDto).ToList();
}
