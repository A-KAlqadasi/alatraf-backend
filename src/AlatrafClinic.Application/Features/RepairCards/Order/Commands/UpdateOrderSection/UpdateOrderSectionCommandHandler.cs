using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;
using AlatrafClinic.Application.Common.Errors;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.UpdateOrderSection;

public sealed class UpdateOrderSectionCommandHandler : IRequestHandler<UpdateOrderSectionCommand, Result<Updated>>
{
    private readonly ILogger<UpdateOrderSectionCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;

    public UpdateOrderSectionCommandHandler(ILogger<UpdateOrderSectionCommandHandler> logger, IAppDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task<Result<Updated>> Handle(UpdateOrderSectionCommand command, CancellationToken ct)
    {
        var repairCard = await _dbContext.RepairCards
            .Include(rc => rc.Orders)
            .SingleOrDefaultAsync(rc => rc.Id == command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("RepairCard with Id {RepairCardId} not found.", command.RepairCardId);
            return RepairCardErrors.InvalidDiagnosisId;
        }

        // Validate Section existence
        var sectionExists = await _dbContext.Sections.AsNoTracking().AnyAsync(s => s.Id == command.SectionId, ct);
        if (!sectionExists)
        {
            _logger.LogError("Section with Id {SectionId} not found.", command.SectionId);
            return ApplicationErrors.SectionNotFound;
        }

        var result = repairCard.UpdateOrderSection(command.OrderId, command.SectionId);
        if (result.IsError)
        {
            _logger.LogError("Failed to update order section for RepairCard {RepairCardId}, Order {OrderId}: {Errors}", command.RepairCardId, command.OrderId, result.Errors);
            return result.Errors;
        }

        _dbContext.RepairCards.Update(repairCard);
        await _dbContext.SaveChangesAsync(ct);

        _logger.LogInformation("Order {OrderId} for RepairCard {RepairCardId} updated to Section {SectionId}.", command.OrderId, command.RepairCardId, command.SectionId);

        return Result.Updated;
    }
}
