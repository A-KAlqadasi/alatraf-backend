using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersBySection;

public sealed class GetOrdersBySectionQueryHandler : IRequestHandler<GetOrdersBySectionQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<GetOrdersBySectionQueryHandler> _logger;

    public GetOrdersBySectionQueryHandler(IUnitOfWork unitOfWork, ILogger<GetOrdersBySectionQueryHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>> Handle(GetOrdersBySectionQuery request, CancellationToken ct)
    {
        var projected = await _unitOfWork.Orders.GetBySectionProjectedAsync(request.SectionId, ct);
        if (projected is null)
        {
            _logger.LogInformation("No orders found for section {SectionId}", request.SectionId);
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>();
        }

        return projected.ToList();
    }
}
