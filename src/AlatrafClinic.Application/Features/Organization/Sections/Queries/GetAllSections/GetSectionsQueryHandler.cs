using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Organization.Sections.Dtos;
using AlatrafClinic.Application.Features.Organization.Sections.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Sections.Queries.GetAllSections;

public sealed class GetSectionsQueryHandler(
    IUnitOfWork unitOfWork
) : IRequestHandler<GetSectionsQuery, Result<List<SectionDto>>>
{
  private readonly IUnitOfWork _unitOfWork = unitOfWork;

  public async Task<Result<List<SectionDto>>> Handle(GetSectionsQuery request, CancellationToken ct)
  {
    var sections = await _unitOfWork.Sections.GetAllSectionsFilteredAsync(
        request.DepartmentId,
        request.IsActiveDoctors,
        request.SearchTerm,
        ct
    );

    return sections.ToDtos();
  }
}