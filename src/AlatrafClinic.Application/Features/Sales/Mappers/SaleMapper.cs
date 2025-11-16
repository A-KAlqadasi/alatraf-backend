using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Domain.Sales;

namespace AlatrafClinic.Application.Features.Sales.Mappers;

public static class SaleMapper
{
    public static SaleDto ToDto(this Sale sale)
    {
        ArgumentNullException.ThrowIfNull(sale);
        return new SaleDto
        {
            SaleId = sale.Id,
            Diagnosis = sale.Diagnosis.ToDto(),
            SaleStatus = sale.Status,
            SaleDate = sale.CreatedAtUtc.DateTime.Date,
            Total = sale.Total,
            SaleItems = sale.SaleItems.ToDtos()
        };
    }
    public static List<SaleDto> ToDtos(this List<Sale> sales)
    {
        ArgumentNullException.ThrowIfNull(sales);
        return sales.Select(sale => sale.ToDto()).ToList();
    }
}