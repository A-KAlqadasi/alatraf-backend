using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRoomsBySectionId;

public sealed record GetRoomsBySectionIdQuery(int SectionId) : IRequest<Result<List<SectionRoomDto>>>;
