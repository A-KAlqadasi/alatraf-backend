using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sections.Commands.DeleteSection;

public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, Result<Deleted>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<DeleteSectionCommandHandler> _logger;

    public DeleteSectionCommandHandler(IAppDbContext context, ILogger<DeleteSectionCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<Deleted>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await _context.Sections.FirstOrDefaultAsync(s=> s.Id == request.SectionId);
        if (section == null)
        {
            return SectionErrors.SectionNotFound;
        }

        _context.Sections.Remove(section);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Section with ID {SectionId} deleted successfully.", request.SectionId);

        return Result.Deleted;
    }
}