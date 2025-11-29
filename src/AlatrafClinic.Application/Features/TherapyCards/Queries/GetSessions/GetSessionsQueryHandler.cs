using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Sessions;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessions;

public sealed class GetSessionsQueryHandler
    : IRequestHandler<GetSessionsQuery, Result<PaginatedList<SessionListDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSessionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<SessionListDto>>> Handle(GetSessionsQuery query, CancellationToken ct)
    {
        var spec = new SessionsFilter(query);

        var totalCount = await _unitOfWork.Sessions.CountAsync(spec, ct);
        
        var sessions = await _unitOfWork.Sessions
            .ListAsync(spec, spec.Page, spec.PageSize, ct);

        var items = sessions
            .Select(s => new SessionListDto
            {
                SessionId = s.Id,
                Number = s.Number,
                IsTaken = s.IsTaken,
                SessionDate = s.SessionDate,

                TherapyCardId = s.TherapyCardId,
                TherapyCardType = s.TherapyCard!.Type.ToString(),
                ProgramStartDate = s.TherapyCard.ProgramStartDate,
                ProgramEndDate = s.TherapyCard.ProgramEndDate,

                PatientId = s.TherapyCard!.Diagnosis!.PatientId,
                PatientName = s.TherapyCard.Diagnosis.Patient!.Person!.FullName,

                SessionPrograms = s.SessionPrograms.ToDtos()
            })
            .ToList();

        return new PaginatedList<SessionListDto>
        {
            Items      = items,
            PageNumber = spec.Page,
            PageSize   = spec.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)spec.PageSize)
        };
    }
}