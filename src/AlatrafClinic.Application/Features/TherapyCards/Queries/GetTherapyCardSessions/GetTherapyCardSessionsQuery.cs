using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardSessions;

public sealed record GetTherapyCardSessionsQuery(int TherapyCardId) : IRequest<Result<List<SessionDto>>>;