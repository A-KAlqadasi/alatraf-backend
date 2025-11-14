using AlatrafClinic.Domain.Inventory.Suppliers;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Mappers;

public static class SupplierMapper
{
    public static SupplierDto ToDto(this Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            SupplierName = supplier.SupplierName,
            Phone = supplier.Phone
        };
    }
    public static List<SupplierDto> ToDtoList(this IEnumerable<Supplier> suppliers)
    {
        if (suppliers == null)
            throw new ArgumentNullException(nameof(suppliers));

        return suppliers.Select(s => s.ToDto()).ToList();
    }
}