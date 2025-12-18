using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.WoundedCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.WoundedCards.Commands.DeleteWoundedCard;

public class DeleteWoundedCardCommandHandler : IRequestHandler<DeleteWoundedCardCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteWoundedCardCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public DeleteWoundedCardCommandHandler(ILogger<DeleteWoundedCardCommandHandler>logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<Deleted>> Handle(DeleteWoundedCardCommand command, CancellationToken ct)
    {
        var woundedCard = await _context.WoundedCards.FirstOrDefaultAsync(d=> d.Id == command.WoundedCardId, ct);

        if (woundedCard is null)
        {
            _logger.LogError("Wounded card with ID {WoundedCardId} not found.", command.WoundedCardId);
            return WoundedCardErrors.WoundedCardNotFound;
        }

        _context.WoundedCards.Remove(woundedCard);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Wounded card with ID {WoundedCardId} deleted successfully.", command.WoundedCardId);

        return Result.Deleted;
    }
}