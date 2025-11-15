using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHolidays;

public sealed record GetHolidaysQuery(
    bool? IsActive = null,
    DateTime? SpecificDate = null,
    DateTime? EndDate = null,
    HolidayType? Type = null,
    string? SortBy = null,
    string? SortDirection = "asc" // "asc" or "desc"
) : ICachedQuery<Result<List<HolidayDto>>>
{
    public string CacheKey =>
        $"holidays:" +
        $"active={(IsActive.HasValue ? IsActive.Value.ToString().ToLower() : "all")}:" +
        $"date={(SpecificDate?.ToString("yyyy-MM-dd") ?? "all")}:" +
        $"enddate={(EndDate?.ToString("yyyy-MM-dd") ?? "all")}:" +
        $"type={(Type?.ToString().ToLower() ?? "all")}:" +
        $"sort={SortBy?.Trim().ToLower() ?? "none"}:" +
        $"dir={SortDirection?.Trim().ToLower() ?? "asc"}";

    public string[] Tags => ["holidays"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}