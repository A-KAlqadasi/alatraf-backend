using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSectionById;

public class GetSectionByIdQueryHandler : IRequestHandler<GetSectionByIdQuery, Result<SectionDto>>
{
    private readonly ILogger<GetSectionByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetSectionByIdQueryHandler(ILogger<GetSectionByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<SectionDto>> Handle(GetSectionByIdQuery query, CancellationToken ct)
    {
        var section = await _unitOfWork.Sections.GetByIdAsync(query.SectionId, ct);
        if (section is null)
        {
            _logger.LogWarning("Section with ID {SectionId} not found.", query.SectionId);
            return SectionErrors.SectionNotFound;
        }

        return section.ToDto();
    }
}