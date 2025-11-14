
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.CreateMedicalProgram;

public class CreateMedicalProgramCommandHandler : IRequestHandler<CreateMedicalProgramCommand, Result<MedicalProgramDto>>
{
    private readonly ILogger<CreateMedicalProgramCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;

    public CreateMedicalProgramCommandHandler(ILogger<CreateMedicalProgramCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
    }
    public async Task<Result<MedicalProgramDto>> Handle(CreateMedicalProgramCommand command, CancellationToken ct)
    {
        var isExists = await _unitOfWork.MedicalPrograms.IsExistsByName(command.Name, ct);
        if (isExists)
        {
            _logger.LogWarning("Medical program {name} already exists", command.Name);
            return MedicalProgramErrors.NameAlreadyExists;
        }

        var medicalProgramResult = MedicalProgram.Create(command.Name, command.Description, command.SectionId);

        if (medicalProgramResult.IsError)
        {
            _logger.LogError("Failed to create medical program: {Error}", medicalProgramResult.Errors);
            return medicalProgramResult.TopError;
        }

        var medicalProgram = medicalProgramResult.Value;

        await _unitOfWork.MedicalPrograms.AddAsync(medicalProgram, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return medicalProgram.ToDto();
    }
}