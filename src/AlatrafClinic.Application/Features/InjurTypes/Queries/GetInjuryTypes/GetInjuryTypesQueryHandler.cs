
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.InjurTypes.Queries.GetInjuryTypes;

public class GetInjuryTypesQueryHandler : IRequestHandler<GetInjuryTypesQuery, Result<List<InjuryDto>>>
{
    private readonly IAppDbContext _context;

    public GetInjuryTypesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<InjuryDto>>> Handle(GetInjuryTypesQuery query, CancellationToken ct)
    {
        var injuryTypes = await _context.InjuryTypes.AsNoTracking().ToListAsync();

        if(injuryTypes is null || injuryTypes.Count == 0)
        {
            return Error.NotFound(code:"InjuryType.NotFound", description:"No injury types found!");
        }

        return injuryTypes.ToDtos();
    }
}