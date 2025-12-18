using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections.Rooms;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Rooms.Commands.CreateRoom;

public sealed class CreateRoomCommandHandler(
    IAppDbContext _context,
    ILogger<CreateRoomCommandHandler> _logger,
    HybridCache _cache
) : IRequestHandler<CreateRoomCommand, Result<RoomDto>>
{
    public async Task<Result<RoomDto>> Handle(CreateRoomCommand command, CancellationToken ct)
    {
        var section = await _context.Sections.Include(s=> s.Department).FirstOrDefaultAsync(s => s.Id == command.SectionId, ct);
        
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

        var isRoomExists = await _context.Sections
            .Where(s => s.Id == command.SectionId)
            .SelectMany(s => s.Rooms)
            .AnyAsync(r => r.Name == command.Name, ct);
        if (isRoomExists)
        {
            _logger.LogError(" Room with name {RoomName} already exists in Section {SectionId}.",
                command.Name, command.SectionId);
            return RoomErrors.DuplicateRoomName;
        }


        await _context.Rooms.AddAsync(room, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("room", ct);

        _logger.LogInformation(" Room created successfully for Section {SectionId}.",
             command.SectionId);

        return room.ToDto();
    }
}