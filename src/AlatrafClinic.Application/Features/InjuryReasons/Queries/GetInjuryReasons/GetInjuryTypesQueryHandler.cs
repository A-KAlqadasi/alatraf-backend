using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.InjuryReasons.Queries.GetInjuryReasons;

public class GetInjuryReasonsQueryHandler : IRequestHandler<GetInjuryReasonsQuery, Result<List<InjuryDto>>>
{
    private readonly IAppDbContext _context;

    public GetInjuryReasonsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<InjuryDto>>> Handle(GetInjuryReasonsQuery query, CancellationToken ct)
    {
        var injuryReasons = await _context.InjuryReasons.AsNoTracking().ToListAsync();

        if(injuryReasons is null || injuryReasons.Count == 0)
        {
            return Error.NotFound(code:"InjuryReason.NotFound", description:"No injury reasons found!");
        }

        return injuryReasons.ToDtos();
    }
}