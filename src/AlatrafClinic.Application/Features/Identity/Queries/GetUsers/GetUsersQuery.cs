namespace AlatrafClinic.Application.Features.Identity.Queries.GetUsers;

using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

public sealed record GetUsersQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    string? UserName = null,
    string? FullName = null,
    bool? IsActive = null,
    string SortColumn = "UserName",
    string SortDirection = "asc"
) : IRequest<Result<PaginatedList<UserDto>>>;