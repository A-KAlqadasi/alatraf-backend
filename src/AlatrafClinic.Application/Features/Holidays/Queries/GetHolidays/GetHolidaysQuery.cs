using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Holidays;

using MediatR;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHolidays;

public sealed record GetHolidaysQuery(
    int Page,
    int PageSize,
    bool? IsActive = null,
    DateOnly? SpecificDate = null,
    DateOnly? EndDate = null,
    HolidayType? Type = null,
    string? SortBy = null,
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<HolidayDto>>>;
// {
//     public string CacheKey =>
//         $"holidays:" +
//         $"p={Page}:ps={PageSize}:" +
//         $"active={(IsActive.HasValue ? IsActive.Value.ToString().ToLower() : "all")}:" +
//         $"date={(SpecificDate?.ToString("yyyy-MM-dd") ?? "all")}:" +
//         $"enddate={(EndDate?.ToString("yyyy-MM-dd") ?? "all")}:" +
//         $"type={(Type?.ToString().ToLower() ?? "all")}:" +
//         $"sort={SortBy?.Trim().ToLower() ?? "startdate"}:" +
//         $"dir={SortDirection.Trim().ToLower()}";

//     public string[] Tags => ["holiday"];

//     public TimeSpan Expiration => TimeSpan.FromMinutes(10);
// }
