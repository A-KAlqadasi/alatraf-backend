using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;

public sealed record class GenerateSessionsCommand(int TherapyCardId) : IRequest<Result<List<SessionDto>>>;