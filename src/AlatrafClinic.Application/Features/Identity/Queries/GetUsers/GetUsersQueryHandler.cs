namespace AlatrafClinic.Application.Features.Identity.Queries.GetUsers;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;


public sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, Result<PaginatedList<UserDto>>>
{
    private readonly IIdentityService _identityService;

    public GetUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<PaginatedList<UserDto>>> Handle(
        GetUsersQuery query,
        CancellationToken ct)
    {
        IQueryable<UserDto> usersQuery = await _identityService.GetUsersAsync();

        usersQuery = ApplyFilters(usersQuery, query);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            usersQuery = ApplySearchTerm(usersQuery, query.SearchTerm);
        }

        usersQuery = ApplySorting(usersQuery, query.SortColumn, query.SortDirection);

        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 10 : query.PageSize;

        var count = await usersQuery.CountAsync(ct);

        var items = await usersQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var result = new PaginatedList<UserDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };

        return result;
    }

    private static IQueryable<UserDto> ApplyFilters(
        IQueryable<UserDto> query,
        GetUsersQuery searchQuery)
    {
        if (!string.IsNullOrWhiteSpace(searchQuery.UserName))
        {
            var normalized = searchQuery.UserName.Trim().ToLower();
            query = query.Where(u =>
                u.UserName != null &&
                u.UserName.ToLower().Contains(normalized));
        }

        if (!string.IsNullOrWhiteSpace(searchQuery.FullName))
        {
            var normalized = searchQuery.FullName.Trim().ToLower();
            query = query.Where(u =>
                u.Person != null &&
                u.Person.Fullname.ToLower().Contains(normalized));
        }

        if (searchQuery.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == searchQuery.IsActive.Value);
        }

        return query;
    }

    private static IQueryable<UserDto> ApplySearchTerm(
        IQueryable<UserDto> query,
        string searchTerm)
    {
        var normalized = searchTerm.Trim().ToLower();

        return query.Where(u =>
            (u.UserName != null &&
             u.UserName.ToLower().Contains(normalized)) ||
            (u.Person != null && (
                u.Person.Fullname.ToLower().Contains(normalized) ||
                (u.Person.Phone ?? "").ToLower().Contains(normalized) ||
                (u.Person.NationalNo ?? "").ToLower().Contains(normalized)
            )));
    }

    private static IQueryable<UserDto> ApplySorting(
        IQueryable<UserDto> query,
        string sortColumn,
        string sortDirection)
    {
        var col = sortColumn?.Trim().ToLowerInvariant() ?? "username";
        var isDescending = sortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

        return col switch
        {
            "fullname" => isDescending
                ? query.OrderByDescending(u => u.Person!.Fullname)
                : query.OrderBy(u => u.Person!.Fullname),

            "isactive" => isDescending
                ? query.OrderByDescending(u => u.IsActive)
                : query.OrderBy(u => u.IsActive),

            "username" or _ => isDescending
                ? query.OrderByDescending(u => u.UserName)
                : query.OrderBy(u => u.UserName),
        };
    }
}
