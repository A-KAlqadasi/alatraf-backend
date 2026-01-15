using Microsoft.Extensions.Logging;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;
using AlatrafClinic.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using AlatrafClinic.Domain.Sessions;


namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessionById;

public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, Result<SessionDto>>
{
    private readonly ILogger<GetSessionByIdQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetSessionByIdQueryHandler(ILogger<GetSessionByIdQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<SessionDto>> Handle(GetSessionByIdQuery query, CancellationToken ct)
    {
        var session = await _context.Sessions
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DiagnosisProgram)
                    .ThenInclude(dp => dp.MedicalProgram)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Section)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Room)
            .Include(s => s.SessionPrograms)
                .ThenInclude(sp => sp.DoctorSectionRoom)
                    .ThenInclude(dsr => dsr!.Doctor)
                        .ThenInclude(d => d.Person)
            .AsNoTracking()
            .FirstOrDefaultAsync(s=> s.Id == query.SessionId, ct);
            
        if (session is null)
        {
            _logger.LogWarning("Session with ID {SessionId} not found.", query.SessionId);
            return SessionErrors.SessionNotFound;
        }

        return session.ToDto();
    }
}