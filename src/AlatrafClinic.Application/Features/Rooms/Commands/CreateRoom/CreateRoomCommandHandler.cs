using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateRoomCommandHandler> logger,

ICacheService cacheService
) : IRequestHandler<CreateRoomCommand, Result<RoomDto>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<CreateRoomCommandHandler> _logger = logger;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<Result<RoomDto>> Handle(CreateRoomCommand command, CancellationToken ct)
    {
        var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, ct);
        
        if (section is null)
        {
            _logger.LogError(" Section {SectionId} not found when creating rooms.", command.SectionId);
            return ApplicationErrors.SectionNotFound;
        }

        var createRoom = Room.Create(command.Name, command.SectionId);

        if(createRoom.IsError)
        {
            _logger.LogError(" Failed to create room in Section {SectionId}. Error: {Error}",
                command.SectionId, createRoom.TopError.ToString());
            return createRoom.Errors;
        }
        var room = createRoom.Value;

        var isRoomExists = await _unitOfWork.Sections.IsSectionHasRoomNameAsync(section.Id, command.Name, ct);
        if (isRoomExists)
        {
            _logger.LogError(" Room with name {RoomName} already exists in Section {SectionId}.",
                command.Name, command.SectionId);
            return RoomErrors.DuplicateRoomName;
        }


        
        await _unitOfWork.Rooms.AddAsync(room, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation(" Room created successfully for Section {SectionId}.",
             command.SectionId);
        await _cacheService.RemoveByTagAsync("room", ct);

        return room.ToDto();
    }
}