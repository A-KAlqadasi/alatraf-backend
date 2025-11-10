using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetSessionById;

public sealed record GetSessionByIdQuery(int SessionId) : IRequest<Result<SessionDto>>;