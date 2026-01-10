using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetDraftOrders;

public sealed class GetDraftOrdersQueryHandler : IRequestHandler<GetDraftOrdersQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetDraftOrdersQueryHandler> _logger;

    public GetDraftOrdersQueryHandler(IAppDbContext dbContext, ILogger<GetDraftOrdersQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>> Handle(GetDraftOrdersQuery request, CancellationToken ct)
    {
        var projected = await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Status == AlatrafClinic.Domain.RepairCards.Enums.OrderStatus.Draft)
            .Select(o => new AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto
            {
                Id = o.Id,
                RepairCardId = o.RepairCardId,
                SectionId = o.SectionId,
                Status = o.Status
            })
            .ToListAsync(ct);
        if (projected is null)
        {
            _logger.LogInformation("No draft orders found.");
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>();
        }

        return projected.ToList();
    }
}
