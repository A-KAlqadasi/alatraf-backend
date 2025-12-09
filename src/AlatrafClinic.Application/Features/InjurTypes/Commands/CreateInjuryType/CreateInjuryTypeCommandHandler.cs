
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.CreateInjuryType;

public class CreateInjuryTypeCommandHandler : IRequestHandler<CreateInjuryTypeCommand, Result<InjuryDto>>
{
    private readonly ILogger<CreateInjuryTypeCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public CreateInjuryTypeCommandHandler(ILogger<CreateInjuryTypeCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<InjuryDto>> Handle(CreateInjuryTypeCommand command, CancellationToken ct)
    {
        var isExists = await _context.InjuryTypes
            .AnyAsync(x => x.Name.ToLower() == command.Name.ToLower(), ct);
        if (isExists)
        {
            _logger.LogWarning("Injury type with name {InjuryTypeName} already exists.", command.Name);
            return Error.Conflict(code: "InjuryType.AlreadyExists", description: "Injury type with the same name already exists.");
        }

        var injuryTypeResult = InjuryType.Create(command.Name);

        if(injuryTypeResult.IsError)
        {
            _logger.LogError("Failed to create injury type with name {InjuryTypeName}. Error: {Error}", command.Name, injuryTypeResult.TopError);
            return Error.Failure(code: "InjuryType.CreationFailed", description: "Failed to create injury type.");
        }
        var injuryType = injuryTypeResult.Value;

        await _context.InjuryTypes.AddAsync(injuryType, ct);
        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Injury type with name {InjuryTypeName} created successfully.", command.Name);

        await _cache.RemoveByTagAsync("injury-type", ct);

        return injuryType.ToDto();
    }
}