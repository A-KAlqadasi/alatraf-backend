using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetLastActiveTherapyCard;

public sealed record class GetLastActiveTherapyCardQuery(
    int PatientId) : IRequest<Result<TherapyCardDto>>;