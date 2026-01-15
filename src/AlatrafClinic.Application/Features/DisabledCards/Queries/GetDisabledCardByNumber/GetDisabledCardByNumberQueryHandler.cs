using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Application.Features.DisabledCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.DisabledCards;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCardByNumber;

public class GetDisabledCardByNumberQueryHandler : IRequestHandler<GetDisabledCardByNumberQuery, Result<DisabledCardDto>>
{
    private readonly ILogger<GetDisabledCardByNumberQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetDisabledCardByNumberQueryHandler(ILogger<GetDisabledCardByNumberQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<DisabledCardDto>> Handle(GetDisabledCardByNumberQuery query, CancellationToken ct)
    {
        var card = await _context.DisabledCards
        .Include(c=> c.Patient)
            .ThenInclude(p=> p.Person)
                .ThenInclude(a=> a.Address)
            .FirstOrDefaultAsync(c=> c.CardNumber == query.CardNumber, ct);

        if (card is null)
        {
            _logger.LogError("Disabled card with number {number} is not found", query.CardNumber);
            return DisabledCardErrors.DisabledCardNotFound;
        }

        return card.ToDto();
    }
}