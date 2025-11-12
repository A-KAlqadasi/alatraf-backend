using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.RepairCards;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCardById;

public class GetRepairCardByIdQueryHandler : IRequestHandler<GetRepairCardByIdQuery, Result<RepairCardDto>>
{
    private readonly ILogger<GetRepairCardByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetRepairCardByIdQueryHandler(ILogger<GetRepairCardByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<RepairCardDto>> Handle(GetRepairCardByIdQuery query, CancellationToken ct)
    {
        var repairCard = await _unitOfWork.RepairCards.GetByIdAsync(query.RepairCardId, ct);
        if (repairCard is null)
        {
            _logger.LogError("Repair card with ID {RepairCardId} not found.", query.RepairCardId);
            return RepairCardErrors.RepairCardNotFound;
        }

        return repairCard.ToDto();
    }
}