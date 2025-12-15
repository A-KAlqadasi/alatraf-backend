namespace AlatrafClinic.Application.Features.Identity.Queries.GetUsers;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;


public sealed record GetUsersQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string? UserName = null,
    string? FullName = null,
    bool? IsActive = null,
    string SortColumn = "UserName",
    string SortDirection = "asc"
) : ICachedQuery<Result<PaginatedList<UserDto>>>
{
    public string CacheKey =>
        $"users:p={Page}:ps={PageSize}" +
        $":q={SearchTerm ?? "-"}" +
        $":uname={UserName ?? "-"}" +
        $":fname={FullName ?? "-"}" +
        $":active={(IsActive?.ToString() ?? "-")}" +
        $":sort={SortColumn}:{SortDirection}";

    public string[] Tags => ["user"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
