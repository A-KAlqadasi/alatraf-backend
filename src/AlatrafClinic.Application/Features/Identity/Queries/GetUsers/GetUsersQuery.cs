using AlatrafClinic.Application.Features.Identity.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Identity.Queries.GetUsers;

public sealed record GetUsersQuery
    : IRequest<Result<IReadOnlyList<UserListItemDto>>>;
