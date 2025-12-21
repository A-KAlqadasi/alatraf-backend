using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Payments;

using MediatR;

namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayments;

public sealed record GetPaymentsQuery(
    int Page,
    int PageSize,
    string? SearchTerm = null,
    int? TicketId = null,
    int? DiagnosisId = null,
    PaymentReference? PaymentReference = null,
    AccountKind? AccountKind = null,
    bool? IsCompleted = null,
    DateTime? PaymentDateFrom = null,
    DateTime? PaymentDateTo = null,
    string SortColumn = "PaymentDate",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<PaymentListItemDto>>>;
