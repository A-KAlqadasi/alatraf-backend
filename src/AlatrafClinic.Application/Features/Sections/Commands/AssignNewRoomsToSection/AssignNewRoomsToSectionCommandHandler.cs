using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sections.Commands.AssignNewRoomsToSection;

public sealed record AssignNewRoomsToSectionCommandHandler : IRequestHandler<AssignNewRoomsToSectionCommand, Result<Success>>
{
    private readonly ILogger<AssignNewRoomsToSectionCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public AssignNewRoomsToSectionCommandHandler(ILogger<AssignNewRoomsToSectionCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<Success>> Handle(AssignNewRoomsToSectionCommand command, CancellationToken ct)
    {
        var section = await _context.Sections
        .Include(s => s.Rooms)
        .FirstOrDefaultAsync(s => s.Id == command.SectionId, ct);
        if (section is null)
        {
            _logger.LogWarning("Section with Id {SectionId} not found.", command.SectionId);
            return SectionErrors.SectionNotFound;
        }
        var result = section.AddRooms(command.RoomNames);
        if (result.IsError)
        {
            _logger.LogWarning("Failed to add rooms to section with Id {SectionId}. Error: {Error}", command.SectionId, result.TopError);
            return result.TopError;
        }

        _context.Sections.Update(section);
        await _context.SaveChangesAsync(ct);

        return Result.Success;
    }
}