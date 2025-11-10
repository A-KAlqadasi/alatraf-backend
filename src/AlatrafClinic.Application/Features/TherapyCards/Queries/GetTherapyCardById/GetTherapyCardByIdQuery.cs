using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;

public sealed record GetTherapyCardByIdQuery(int TherapyCardId)
    : ICachedQuery<Result<TherapyCardDto>>
{
    public string CacheKey => $"therapycard:{TherapyCardId}";
    public string[] Tags => ["therapycard"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(15);
}