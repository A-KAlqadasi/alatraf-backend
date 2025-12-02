
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Rooms.Queries.GetRoomById;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, Result<RoomDto>>
{
    private readonly ILogger<GetRoomByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetRoomByIdQueryHandler(ILogger<GetRoomByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<RoomDto>> Handle(GetRoomByIdQuery query, CancellationToken ct)
    {
        var room =  await _unitOfWork.Rooms.GetByIdAsync(query.RoomId, ct);
        if (room is null)
        {
            _logger.LogError(" Room {RoomId} not found.", query.RoomId);
            return RoomErrors.NotFound;
        }

        return room.ToDto();
    }
}