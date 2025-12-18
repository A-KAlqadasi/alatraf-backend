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
    PaymentReference? PaymentReference = null,
    AccountKind? AccountKind = null,
    bool? IsCompleted = null,
    int? DiagnosisId = null,
    int? TicketId = null,
    int? PatientId = null,
    string SortColumn = "CreatedAtUtc",
    string SortDirection = "desc"
) : IRequest<Result<PaginatedList<PaymentDto>>>;