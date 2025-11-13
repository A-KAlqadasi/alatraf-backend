using Microsoft.Extensions.Logging;
using MediatR;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards;

namespace AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;

public class GetTherapyCardByIdQueryHandler
    : IRequestHandler<GetTherapyCardByIdQuery, Result<TherapyCardDto>>
{
    private readonly ILogger<GetTherapyCardByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetTherapyCardByIdQueryHandler(ILogger<GetTherapyCardByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TherapyCardDto>> Handle(GetTherapyCardByIdQuery query, CancellationToken ct)
    {
        var card = await _unitOfWork.TherapyCards.GetByIdAsync(query.TherapyCardId, ct);

        // var card = await therapyQuery
        //     .Include(tc => tc.Diagnosis)!
        //         .ThenInclude(d => d.Patient)!.ThenInclude(p => p.Person)
        //     .Include(tc => tc.DiagnosisPrograms)!.ThenInclude(dp => dp.MedicalProgram)
        //     .Include(tc => tc.Sessions)!
        //         .ThenInclude(s => s.SessionPrograms)!
        //             .ThenInclude(sp => sp.DiagnosisProgram)!.ThenInclude(dp => dp.MedicalProgram)
        //     .Include(tc => tc.Sessions)!
        //         .ThenInclude(s => s.SessionPrograms)!
        //             .ThenInclude(sp => sp.DoctorSectionRoom)
        //     .FirstOrDefaultAsync(tc => tc.Id == query.TherapyCardId, ct);

        if (card is null)
        {
            _logger.LogWarning("Therapy card with ID {TherapyCardId} not found.", query.TherapyCardId);
            return TherapyCardErrors.TherapyCardNotFound;
        }

        return card.ToDto();
    }
}