using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Application.Features.Payments.Dtos;
using AlatrafClinic.Application.Features.Payments.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;


namespace AlatrafClinic.Application.Features.Payments.Queries.GetPayments;

public sealed class GetPaymentsQueryHandler
    : IRequestHandler<GetPaymentsQuery, Result<PaginatedList<PaymentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPaymentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<PaymentDto>>> Handle(GetPaymentsQuery query, CancellationToken ct)
    {
        var spec = new PaymentsFilter(query);

        var totalCount = await _unitOfWork.Payments.CountAsync(spec, ct);

        // page data
        var payments = await _unitOfWork.Payments
            .ListAsync(spec, spec.Page, spec.PageSize, ct);

        var items = payments
            .Select(p => p.ToDto())
            .ToList();

        return new PaginatedList<PaymentDto>
        {
            Items      = items,
            PageNumber = spec.Page,
            PageSize   = spec.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)spec.PageSize)
        };
    }
}