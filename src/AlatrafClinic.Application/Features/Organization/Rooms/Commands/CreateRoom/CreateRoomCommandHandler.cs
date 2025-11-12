using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Organization.Rooms.Dtos;
using AlatrafClinic.Application.Features.Organization.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Organization.Rooms;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Organization.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateRoomCommandHandler> logger,

ICacheService cacheService
) : IRequestHandler<CreateRoomCommand, Result<List<RoomDto>>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;
  private readonly ILogger<CreateRoomCommandHandler> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<List<RoomDto>>> Handle(CreateRoomCommand request, CancellationToken ct)
  {
    var section = await _unitOfWork.Sections.GetByIdAsync(request.SectionId, ct);
    if (section is null)
    {
      _logger.LogWarning(" Section {SectionId} not found when creating rooms.", request.SectionId);
      return ApplicationErrors.SectionNotFound;
    }

    var createdRooms = new List<Room>();

    foreach (var name in request.RoomNames)
    {
        var roomResult = section.AddRoom(name);
      
        if (roomResult.IsError)
        {
            _logger.LogWarning(" Failed to create room {RoomName} for Section {SectionId}: {Error}",
                name, request.SectionId, roomResult.Errors);
            return roomResult.Errors;
        }

      createdRooms.Add(roomResult.Value);
    }

    await _unitOfWork.Rooms.AddRangeAsync(createdRooms, ct);
    await _unitOfWork.SaveChangesAsync(ct);

    _logger.LogInformation(" {Count} room(s) created successfully for Section {SectionId}.",
        createdRooms.Count, request.SectionId);
    await _cacheService.RemoveByTagAsync("room", ct);

    return createdRooms.ToDtos();
  }
}