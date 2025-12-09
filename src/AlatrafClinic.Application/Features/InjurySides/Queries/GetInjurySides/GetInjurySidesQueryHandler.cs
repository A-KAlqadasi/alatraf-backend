using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.InjurySides.Queries.GetInjurySides;

public class GetInjurySidesQueryHandler : IRequestHandler<GetInjurySidesQuery, Result<List<InjuryDto>>>
{
    private readonly IAppDbContext _context;

    public GetInjurySidesQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<InjuryDto>>> Handle(GetInjurySidesQuery query, CancellationToken ct)
    {
        var injurySides = await _context.InjurySides.AsNoTracking().ToListAsync();

        if(injurySides is null || injurySides.Count == 0)
        {
            return Error.NotFound(code:"InjurySide.NotFound", description:"No injury sides found!");
        }

        return injurySides.ToDtos();
    }
}