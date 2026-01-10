// Application/Sagas/SaleSagaResult.cs
using System.Collections.Generic;

namespace AlatrafClinic.Application.Sagas
{
    public sealed class SaleSagaResult
    {
        public bool IsSuccess { get; init; }
        public List<string> Errors { get; init; } = new();
        public int? SaleId { get; init; }
        public decimal Total { get; init; }

        public static SaleSagaResult Ok(int saleId, decimal total) 
            => new() { IsSuccess = true, SaleId = saleId, Total = total };

        public static SaleSagaResult Success() 
            => new() { IsSuccess = true };

        public static SaleSagaResult Fail(params string[] errors) 
            => new() { IsSuccess = false, Errors = new List<string>(errors) };
    }
}