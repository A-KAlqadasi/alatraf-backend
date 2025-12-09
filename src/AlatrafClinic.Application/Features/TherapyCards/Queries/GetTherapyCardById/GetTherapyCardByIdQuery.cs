using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;

public sealed record GetTherapyCardByIdQuery(int TherapyCardId)
    : ICachedQuery<Result<TherapyCardDiagnosisDto>>
{
    public string CacheKey => $"therapycard:{TherapyCardId}";
    public string[] Tags => ["therapy-card"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(15);
}
