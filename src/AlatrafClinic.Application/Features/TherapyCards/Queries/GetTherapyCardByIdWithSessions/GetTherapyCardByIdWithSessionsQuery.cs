using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;


namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardByIdWithSessions;

public sealed record GetTherapyCardByIdWithSessionsQuery(int TherapyCardId)
    : ICachedQuery<Result<TherapyCardDto>>
{
    public string CacheKey => $"therapycard:{TherapyCardId}";
    public string[] Tags => ["therapy-card"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(15);
}