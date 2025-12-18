using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPaymentsWaitingList;

public sealed record GetPaymentsWaitingListQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    PaymentReference? PaymentReference = null,
    bool? IsCompleted = null,
    string SortColumn = "CreatedAtUtc",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<PaymentWaitingListDto>>>;