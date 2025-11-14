namespace AlatrafClinic.Application.Features.Inventory.Items.Dtos;

public class ItemUnitDto
{
    public int UnitId { get; set; }
    public decimal Price { get; set; }
    public decimal ConversionFactor { get; set; } = 1;
    public decimal? MinPriceToPay { get; set; }
    public decimal? MaxPriceToPay { get; set; }
}
