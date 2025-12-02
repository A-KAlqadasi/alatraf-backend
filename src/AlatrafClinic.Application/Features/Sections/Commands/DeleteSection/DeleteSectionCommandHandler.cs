using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sections.Commands.DeleteSection;

public class DeleteSectionCommandHandler : IRequestHandler<DeleteSectionCommand, Result<Deleted>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteSectionCommandHandler> _logger;

    public DeleteSectionCommandHandler(IUnitOfWork unitOfWork, ILogger<DeleteSectionCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Deleted>> Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var section = await _unitOfWork.Sections.GetByIdAsync(request.SectionId);
        if (section == null)
        {
            return SectionErrors.SectionNotFound;
        }

        await _unitOfWork.Sections.DeleteAsync(section);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Section with ID {SectionId} deleted successfully.", request.SectionId);

        return Result.Deleted;
    }
}