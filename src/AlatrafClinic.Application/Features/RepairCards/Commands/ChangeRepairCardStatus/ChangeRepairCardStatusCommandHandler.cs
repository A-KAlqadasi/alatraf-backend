using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;
using AlatrafClinic.Domain.RepairCards.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Commands.ChangeRepairCardStatus;

public class ChangeRepairCardStatusCommandHandler : IRequestHandler<ChangeRepairCardStatusCommand, Result<Updated>>
{
    private readonly ILogger<ChangeRepairCardStatusCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public ChangeRepairCardStatusCommandHandler(ILogger<ChangeRepairCardStatusCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<Updated>> Handle(ChangeRepairCardStatusCommand command, CancellationToken ct)
    {
        var repairCard = await _context.RepairCards.FirstOrDefaultAsync(r=> r.Id == command.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("Repair card with ID {RepairCardId} not found.", command.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }
        Result<Updated> result;

        switch (command.NewStatus)
        {
            case RepairCardStatus.InProgress:
                result = repairCard.MarkAsInProgress();
                break;
            case RepairCardStatus.InTraining:
                result = repairCard.MarkAsInTraining();
                break;
            case RepairCardStatus.Completed:
                result = repairCard.MarkAsCompleted();
                break;
            case RepairCardStatus.ExitForPractice:
                result = repairCard.MarkAsExitForPractice();
                break;
            case RepairCardStatus.IllegalExit:
                result = repairCard.MarkAsIllegalExit();
                break;
            case RepairCardStatus.LegalExit:
                result = repairCard.MarkAsLegalExit();
                break;
            default:
                _logger.LogError("Invalid status {NewStatus} provided for Repair card ID {RepairCardId}.", command.NewStatus, command.RepairCardId);
                return RepairCardErrors.InvalidStatus;
        }

        if (result.IsError)
        {
            _logger.LogError("Failed to change status of Repair card ID {RepairCardId} to {NewStatus}. Error: {Error}", command.RepairCardId, command.NewStatus, result.TopError);
            return result.Errors;
        }
        
        _context.RepairCards.Update(repairCard);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Successfully changed status of Repair card ID {RepairCardId} to {NewStatus}.", command.RepairCardId, command.NewStatus);

        return Result.Updated;
    }
}