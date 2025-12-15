// using AlatrafClinic.Application.Common.Interfaces;
// using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;
// using AlatrafClinic.Domain.Common.Results;

// namespace AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoresWithItems;

// public sealed record GetStoresWithItemsQuery : ICachedQuery<Result<List<StoreWithItemsDto>>>
// {
//     public string CacheKey => "stores_with_items";
//     public string[] Tags => ["stores", "store_items"];
//     public TimeSpan Expiration => TimeSpan.FromMinutes(10);
// }
// // 