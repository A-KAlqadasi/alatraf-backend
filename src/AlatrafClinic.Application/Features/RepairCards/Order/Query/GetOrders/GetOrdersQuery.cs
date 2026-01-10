using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrders;

public sealed record GetOrdersQuery(
    int? SectionId,
    int? RepairCardId,
    string? Status,
    string? SearchTerm,
    string? SortColumn,
    string? SortDirection,
    int Page = 1,
    int PageSize = 10
) : IRequest<Result<AlatrafClinic.Application.Common.Models.PaginatedList<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>;  
