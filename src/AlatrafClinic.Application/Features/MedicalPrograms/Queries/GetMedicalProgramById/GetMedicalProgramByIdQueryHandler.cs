using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramById;

public class GetMedicalProgramByIdQueryHandler : IRequestHandler<GetMedicalProgramByIdQuery, Result<MedicalProgramDto>>
{
    private readonly ILogger<GetMedicalProgramByIdQueryHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public GetMedicalProgramByIdQueryHandler(ILogger<GetMedicalProgramByIdQueryHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<MedicalProgramDto>> Handle(GetMedicalProgramByIdQuery query, CancellationToken ct)
    {
        var medicalProgram = await _unitOfWork.MedicalPrograms.GetByIdAsync(query.MedicalProgramId, ct);
        if (medicalProgram is null)
        {
            _logger.LogWarning("Medical program not found: {MedicalProgramId}", query.MedicalProgramId);

            return MedicalProgramErrors.MedicalProgramNotFound;
        }
        
        return medicalProgram.ToDto();
    }
}