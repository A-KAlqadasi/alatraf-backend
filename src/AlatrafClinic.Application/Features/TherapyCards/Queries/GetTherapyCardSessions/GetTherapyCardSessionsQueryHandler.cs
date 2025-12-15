
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardSessions;

public class GetTherapyCardSessionsQueryHandler : IRequestHandler<GetTherapyCardSessionsQuery, Result<List<SessionDto>>>
{
    private readonly IAppDbContext _context;

    public GetTherapyCardSessionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<List<SessionDto>>> Handle(GetTherapyCardSessionsQuery query, CancellationToken ct)
    {
        var sessions = await _context.Sessions
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
        .Where(s=> s.TherapyCardId == query.TherapyCardId).ToListAsync(ct);

        return sessions.ToDtos();
    }
}