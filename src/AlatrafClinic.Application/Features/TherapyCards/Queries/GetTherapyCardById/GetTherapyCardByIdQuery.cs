using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;

public sealed record GetTherapyCardByIdQuery(int TherapyCardId)
    : IRequest<Result<TherapyCardDiagnosisDto>>;