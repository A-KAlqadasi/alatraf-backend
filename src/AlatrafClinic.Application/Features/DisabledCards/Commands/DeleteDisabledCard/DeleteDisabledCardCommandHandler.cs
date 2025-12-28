using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.DeleteDisabledCard;

public class DeleteDisabledCardCommandHandler : IRequestHandler<DeleteDisabledCardCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteDisabledCardCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public DeleteDisabledCardCommandHandler(ILogger<DeleteDisabledCardCommandHandler>logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<Result<Deleted>> Handle(DeleteDisabledCardCommand command, CancellationToken ct)
    {
        var disabledCard = await _context.DisabledCards.FirstOrDefaultAsync(d=> d.Id == command.DisabledCardId, ct);

        if (disabledCard is null)
        {
            _logger.LogError("Disabled card with ID {DisabledCardId} not found.", command.DisabledCardId);
            return DisabledCardErrors.DisabledCardNotFound;
        }

        _context.DisabledCards.Remove(disabledCard);
        await _context.SaveChangesAsync(ct);
        
        return Result.Deleted;
    }
}