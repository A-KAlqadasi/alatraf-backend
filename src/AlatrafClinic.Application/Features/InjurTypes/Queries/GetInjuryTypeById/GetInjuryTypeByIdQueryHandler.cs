
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurTypes.Queries.GetInjuryTypeById;

public class GetInjuryTypeByIdQueryHandler : IRequestHandler<GetInjuryTypeByIdQuery, Result<InjuryDto>>
{
    private readonly ILogger<GetInjuryTypeByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetInjuryTypeByIdQueryHandler(ILogger<GetInjuryTypeByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<InjuryDto>> Handle(GetInjuryTypeByIdQuery query, CancellationToken ct)
    {
        var injuryType = await _context.InjuryTypes.FirstOrDefaultAsync(x=> x.Id == query.InjuryTypeId, ct);
        if(injuryType is null)
        {
            _logger.LogError("Injury type with ID {InjuryTypeId} not found.", query.InjuryTypeId);
            return Error.NotFound(code: "InjuryType.NotFound", description: "Injury type not found.");
        }

        return injuryType.ToDto();
    }
}