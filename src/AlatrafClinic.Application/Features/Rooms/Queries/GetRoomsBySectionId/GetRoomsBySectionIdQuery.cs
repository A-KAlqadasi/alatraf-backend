
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;


namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRoomsBySectionId;

public sealed record GetRoomsBySectionIdQuery(int SectionId) : IRequest<Result<List<SectionRoomDto>>>;
// {
//     public string CacheKey => $"GetRoomsBySectionIdQuery_SectionId_{SectionId}";

//     public string[] Tags => new[] { "room" };

//     public TimeSpan Expiration => TimeSpan.FromHours(1);
// }