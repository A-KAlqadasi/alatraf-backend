using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.InjurySides;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.CreateInjurySide;

public class CreateInjurySideCommandHandler : IRequestHandler<CreateInjurySideCommand, Result<InjuryDto>>
{
    private readonly ILogger<CreateInjurySideCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public CreateInjurySideCommandHandler(ILogger<CreateInjurySideCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<InjuryDto>> Handle(CreateInjurySideCommand command, CancellationToken ct)
    {
        var isExists = await _context.InjurySides
            .AnyAsync(x => x.Name.ToLower() == command.Name.ToLower(), ct);
        if (isExists)
        {
            _logger.LogWarning("Injury side with name {InjurySideName} already exists.", command.Name);
            return Error.Conflict(code: "InjurySide.AlreadyExists", description: "Injury side with the same name already exists.");
        }

        var injurySideResult = InjurySide.Create(command.Name);
        if(injurySideResult.IsError)
        {
            _logger.LogError("Failed to create injury side with name {InjurySideName}. Error: {Error}", command.Name, injurySideResult.TopError);
            return Error.Failure(code: "InjurySide.CreationFailed", description: "Failed to create injury side.");
        }
        var injurySide = injurySideResult.Value;

        await _context.InjurySides.AddAsync(injurySide, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("injury-side", ct);

        _logger.LogInformation("Injury side with name {InjurySideName} created successfully.", command.Name);

        return injurySide.ToDto();
    }
}