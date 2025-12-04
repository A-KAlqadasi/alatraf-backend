using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetDraftOrders;

public sealed class GetDraftOrdersQueryHandler : IRequestHandler<GetDraftOrdersQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetDraftOrdersQueryHandler> _logger;

    public GetDraftOrdersQueryHandler(IUnitOfWork unitOfWork, ILogger<GetDraftOrdersQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>> Handle(GetDraftOrdersQuery request, CancellationToken ct)
    {
        var projected = await _unitOfWork.Orders.GetDraftsProjectedAsync(ct);
        if (projected is null)
        {
            _logger.LogInformation("No draft orders found.");
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>();
        }

        return projected.ToList();
    }
}
