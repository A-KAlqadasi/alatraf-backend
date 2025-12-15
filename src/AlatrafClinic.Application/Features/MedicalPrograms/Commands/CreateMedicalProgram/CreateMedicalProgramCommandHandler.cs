
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.CreateMedicalProgram;

public class CreateMedicalProgramCommandHandler : IRequestHandler<CreateMedicalProgramCommand, Result<MedicalProgramDto>>
{
    private readonly ILogger<CreateMedicalProgramCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public CreateMedicalProgramCommandHandler(ILogger<CreateMedicalProgramCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<MedicalProgramDto>> Handle(CreateMedicalProgramCommand command, CancellationToken ct)
    {
        var isExists = await _context.MedicalPrograms.Include(m=> m.Section).AnyAsync(mp => mp.Name == command.Name, ct);
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

        await _context.MedicalPrograms.AddAsync(medicalProgram, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("medical-program");

        _logger.LogInformation("Medical program {name} created successfully", command.Name);

        return medicalProgram.ToDto();
    }
}