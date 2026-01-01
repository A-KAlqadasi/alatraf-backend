using System.Collections.Generic;

namespace AlatrafClinic.Application.Sagas;

public sealed class SaleSagaResult
{
    public bool Success { get; init; }
    public List<string> Errors { get; init; } = new();
    public int? SaleId { get; init; }
    public decimal TotalAmount { get; init; }

    public static SaleSagaResult Ok(int saleId, decimal total) => new() { Success = true, SaleId = saleId, TotalAmount = total };

    public static SaleSagaResult Fail(params string[] errors) => new() { Success = false, Errors = new List<string>(errors) };
}
