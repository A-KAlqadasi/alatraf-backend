
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.CreateDeliveryTime;

public class CreateDeliveryTimeCommandHandler : IRequestHandler<CreateDeliveryTimeCommand, Result<Created>>
{
    private readonly ILogger<CreateDeliveryTimeCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public CreateDeliveryTimeCommandHandler(ILogger<CreateDeliveryTimeCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Created>> Handle(CreateDeliveryTimeCommand command, CancellationToken ct)
    {
        RepairCard? repairCard = await _context.RepairCards.FirstOrDefaultAsync(r=> r.Id == command.RepairCardId, ct);

        if (repairCard is null)
        {
            _logger.LogError("Repair card {repairCardId} not found to create delivery", command.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        var result = repairCard.AssignDeliveryTime(command.DeliveryDate, command.Notes);
        if (result.IsError)
        {
            _logger.LogError("Failed to assign delivery time for repair card {repairCardId}: {error}", command.RepairCardId, string.Join(", ", result.Errors));
            return result.TopError;
        }
        
        _context.RepairCards.Update(repairCard);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("repair-card");

        _logger.LogInformation("Delivery time created for repair card {repairCardId}", command.RepairCardId);

        return Result.Created;
    }
}