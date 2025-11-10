using Microsoft.Extensions.Logging;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.Sessions;

using MediatR;


namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessionById;

public class GetSessionByIdQueryHandler : IRequestHandler<GetSessionByIdQuery, Result<SessionDto>>
{
    private readonly ILogger<GetSessionByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetSessionByIdQueryHandler(ILogger<GetSessionByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<SessionDto>> Handle(GetSessionByIdQuery query, CancellationToken ct)
    {
        var session = await _unitOfWork.Sessions.GetByIdAsync(query.SessionId, ct);
        if (session is null)
        {
            _logger.LogWarning("Session with ID {SessionId} not found.", query.SessionId);
            return SessionErrors.SessionNotFound;
        }

        return session.ToDto();
    }
}