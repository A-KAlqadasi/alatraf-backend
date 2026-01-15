using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetOrdersBySection;

public sealed class GetOrdersBySectionQueryHandler : IRequestHandler<GetOrdersBySectionQuery, Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetOrdersBySectionQueryHandler> _logger;

    public GetOrdersBySectionQueryHandler(IAppDbContext dbContext, ILogger<GetOrdersBySectionQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result<List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>>> Handle(GetOrdersBySectionQuery request, CancellationToken ct)
    {
        var projected = await _dbContext.Orders
            .AsNoTracking()
            .Where(o => o.SectionId == request.SectionId)
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
            _logger.LogInformation("No orders found for section {SectionId}", request.SectionId);
            return new List<AlatrafClinic.Application.Features.RepairCards.Dtos.OrderDto>();
        }

        return projected.ToList();
    }
}
