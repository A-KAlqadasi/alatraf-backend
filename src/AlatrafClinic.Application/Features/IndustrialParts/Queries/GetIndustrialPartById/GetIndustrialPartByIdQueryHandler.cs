using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartById;

public class GetIndustrialPartByIdQueryHandler : IRequestHandler<GetIndustrialPartByIdQuery, Result<IndustrialPartDto>>
{
    private readonly ILogger<GetIndustrialPartByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetIndustrialPartByIdQueryHandler(ILogger<GetIndustrialPartByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<IndustrialPartDto>> Handle(GetIndustrialPartByIdQuery query, CancellationToken ct)
    {
        var industrialPart = await _unitOfWork.IndustrialParts.GetByIdAsync(query.IdustrialPartId, ct);
        if (industrialPart is null)
        {
            _logger.LogError("Industrial part with ID {IndustrialPartId} not found.", query.IdustrialPartId);

            return IndustrialPartErrors.IndustrialPartNotFound;
        }
        
        return industrialPart.ToDto();
    }
}