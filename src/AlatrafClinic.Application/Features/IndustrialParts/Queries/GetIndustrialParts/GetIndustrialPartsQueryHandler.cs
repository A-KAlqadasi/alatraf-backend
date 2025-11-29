using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialParts;

public sealed class GetIndustrialPartsQueryHandler
    : IRequestHandler<GetIndustrialPartsQuery, Result<PaginatedList<IndustrialPartDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetIndustrialPartsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<IndustrialPartDto>>> Handle(GetIndustrialPartsQuery query, CancellationToken ct)
    {
        var spec = new IndustrialPartsFilter(query);

        // Count for pagination
        var totalCount = await _unitOfWork.IndustrialParts.CountAsync(spec, ct);

        // Data for current page
        var parts = await _unitOfWork.IndustrialParts
            .ListAsync(spec, spec.Page, spec.PageSize, ct);

        var items = parts
            .Select(p => new IndustrialPartDto
            {
                IndustrialPartId = p.Id,
                Name = p.Name,
                Description = p.Description,
                IndustrialPartUnits = p.IndustrialPartUnits.ToDtos()
            })
            .ToList();

        return new PaginatedList<IndustrialPartDto>
        {
            Items = items,
            PageNumber = spec.Page,
            PageSize = spec.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)spec.PageSize)
        };
    }
}
