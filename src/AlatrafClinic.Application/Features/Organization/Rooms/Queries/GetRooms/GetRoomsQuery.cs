using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Organization.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Queries.GetRooms;

public sealed record GetRoomsQuery(
    int? SectionId = null,
    string? SearchTerm = null
) : ICachedQuery<Result<List<RoomDto>>>
{
    public string CacheKey =>
        $"rooms:section={SectionId?.ToString() ?? "all"}:search={SearchTerm?.Trim().ToLower() ?? "all"}";

    public string[] Tags => ["room"];
    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
