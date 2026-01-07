using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Holidays.Queries.GetHoliday;

public sealed record GetHolidayQueryHandler : IRequestHandler<GetHolidayQuery, Result<HolidayDto>>
{
    private readonly ILogger<GetHolidayQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetHolidayQueryHandler(ILogger<GetHolidayQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<HolidayDto>> Handle(GetHolidayQuery query, CancellationToken ct)
    {
        var holiday = await _context.Holidays
            .Where(h => h.Id == query.Id)
            .Select(h => new HolidayDto
            {
                HolidayId = h.Id,
                Name = h.Name,
                StartDate = h.StartDate,
                EndDate = h.EndDate,
                IsRecurring = h.IsRecurring,
                Type = h.Type,
                IsActive = h.IsActive
            })
            .FirstOrDefaultAsync(ct);

        if (holiday == null)
        {
            _logger.LogError("Holiday with id {HolidayId} not found.", query.Id);
            return Error.NotFound($"Holiday with id {query.Id} not found.");
        }
        
        return holiday;
    }
}