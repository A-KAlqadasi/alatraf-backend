
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Services.Queries.GetServices;

public record GetServicesQuery : ICachedQuery<Result<List<ServiceDto>>>
{
    public string CacheKey =>  "get-services";

    public string[] Tags => new[] { "service" };

    public TimeSpan Expiration => TimeSpan.FromHours(1);
}