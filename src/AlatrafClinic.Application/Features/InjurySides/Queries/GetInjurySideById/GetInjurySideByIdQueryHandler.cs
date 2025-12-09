using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurySides.Queries.GetInjurySideById;

public class GetInjurySideByIdQueryHandler : IRequestHandler<GetInjurySideByIdQuery, Result<InjuryDto>>
{
    private readonly ILogger<GetInjurySideByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetInjurySideByIdQueryHandler(ILogger<GetInjurySideByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<InjuryDto>> Handle(GetInjurySideByIdQuery query, CancellationToken ct)
    {
        var injurySide = await _context.InjurySides.FirstOrDefaultAsync(x=> x.Id == query.InjurySideId, ct);
        if(injurySide is null)
        {
            _logger.LogError("Injury side with ID {InjurySideId} not found.", query.InjurySideId);
            return Error.NotFound(code: "InjurySide.NotFound", description: "Injury side not found.");
        }

        return injurySide.ToDto();
    }
}