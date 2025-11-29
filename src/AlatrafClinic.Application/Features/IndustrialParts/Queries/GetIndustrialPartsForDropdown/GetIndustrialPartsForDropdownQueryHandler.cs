using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartsForDropdown;

public sealed class GetIndustrialPartsForDropdownQueryHandler
    : IRequestHandler<GetIndustrialPartsForDropdownQuery, Result<List<IndustrialPartDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetIndustrialPartsForDropdownQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<IndustrialPartDto>>> Handle(
        GetIndustrialPartsForDropdownQuery query,
        CancellationToken ct)
    {
        var data = await _unitOfWork.IndustrialParts.GetAllAsync(ct);
        if(data is null || !data.Any())
        {
            return IndustrialPartErrors.NoIndustrialPartsFound;
        }
        data = data.OrderBy(x => x.Name).ToList();

        return data.ToDtos();
    }
}