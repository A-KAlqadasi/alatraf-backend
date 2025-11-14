namespace AlatrafClinic.Application.Features.Inventory.Items.Dtos;

public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int BaseUnitId { get; set; }
    public string BaseUnitName { get; set; } = string.Empty;

    public List<ItemUnitDto> Units { get; set; } = new();
}
