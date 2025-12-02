using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetSections;

public class GetSectionsQueryHandler
    : IRequestHandler<GetSectionsQuery, Result<PaginatedList<SectionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSectionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<SectionDto>>> Handle(
        GetSectionsQuery query,
        CancellationToken ct)
    {
        var specification = new SectionsFilter(query);

        var totalCount = await _unitOfWork.Sections.CountAsync(specification, ct);

        var sections = await _unitOfWork.Sections
            .ListAsync(specification, specification.Page, specification.PageSize, ct);

        var items = sections.ToDtos();

        return new PaginatedList<SectionDto>
        {
            Items      = items,
            PageNumber = specification.Page,
            PageSize   = specification.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)specification.PageSize)
        };
    }
}
