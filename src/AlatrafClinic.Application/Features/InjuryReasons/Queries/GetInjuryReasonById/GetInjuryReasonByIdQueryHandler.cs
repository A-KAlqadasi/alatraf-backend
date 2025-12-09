using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjuryReasons.Queries.GetInjuryReasonById;

public class GetInjuryReasonByIdQueryHandler : IRequestHandler<GetInjuryReasonByIdQuery, Result<InjuryDto>>
{
    private readonly ILogger<GetInjuryReasonByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetInjuryReasonByIdQueryHandler(ILogger<GetInjuryReasonByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<InjuryDto>> Handle(GetInjuryReasonByIdQuery query, CancellationToken ct)
    {
        var injuryReason = await _context.InjuryReasons.FirstOrDefaultAsync(x=> x.Id == query.InjuryReasonId, ct);
        if(injuryReason is null)
        {
            _logger.LogError("Injury reason with ID {InjuryReasonId} not found.", query.InjuryReasonId);
            return Error.NotFound(code: "InjuryReason.NotFound", description: "Injury reason not found.");
        }

        return injuryReason.ToDto();
    }
}