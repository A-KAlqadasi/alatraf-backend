namespace AlatrafClinic.Application.Common.Models;

public sealed class ItemSearchSpec
{
    public string? Keyword { get; }
    public int? BaseUnitId { get; }
    public int? UnitId { get; }
    public decimal? MinPrice { get; }
    public decimal? MaxPrice { get; }
    public bool? IsActive { get; }
    public string SortBy { get; }
    public string SortDir { get; }
    public int Page { get; }
    public int PageSize { get; }

    public ItemSearchSpec(
        string? keyword,
        int? baseUnitId,
        int? unitId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? isActive,
        string sortBy,
        string sortDir,
        int page,
        int pageSize)
    {
        Keyword = keyword;
        BaseUnitId = baseUnitId;
        UnitId = unitId;
        MinPrice = minPrice;
        MaxPrice = maxPrice;
        IsActive = isActive;
        SortBy = sortBy;
        SortDir = sortDir;
        Page = Math.Max(1, page);
        PageSize = Math.Clamp(pageSize, 5, 100);
    }
}
