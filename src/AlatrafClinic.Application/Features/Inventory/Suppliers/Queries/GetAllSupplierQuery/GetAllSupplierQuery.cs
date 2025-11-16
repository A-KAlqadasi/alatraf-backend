using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetAllSuppliersQuery;

public sealed record GetAllSuppliersQuery : ICachedQuery<Result<List<SupplierDto>>>
{
    public string CacheKey => "suppliers";
    public string[] Tags => ["supplier"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
