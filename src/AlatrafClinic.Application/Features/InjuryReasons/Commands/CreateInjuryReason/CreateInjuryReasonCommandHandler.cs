using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.CreateInjuryReason;

public class CreateInjuryReasonCommandHandler : IRequestHandler<CreateInjuryReasonCommand, Result<InjuryDto>>
{
    private readonly ILogger<CreateInjuryReasonCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public CreateInjuryReasonCommandHandler(ILogger<CreateInjuryReasonCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<InjuryDto>> Handle(CreateInjuryReasonCommand command, CancellationToken ct)
    {
        var isExists = await _context.InjuryReasons
            .AnyAsync(x => x.Name.ToLower() == command.Name.ToLower(), ct);
        if (isExists)
        {
            _logger.LogWarning("Injury reason with name {InjuryReasonName} already exists.", command.Name);
            return Error.Conflict(code: "InjuryReason.AlreadyExists", description: "Injury reason with the same name already exists.");
        }

        var injuryReasonResult = InjuryReason.Create(command.Name);
        if(injuryReasonResult.IsError)
        {
            _logger.LogError("Failed to create injury reason with name {InjuryReasonName}. Error: {Error}", command.Name, injuryReasonResult.TopError);
            return Error.Failure(code: "InjuryReason.CreationFailed", description: "Failed to create injury reason.");
        }
        var injuryReason = injuryReasonResult.Value;

        await _context.InjuryReasons.AddAsync(injuryReason, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("injury-reason", ct);

        _logger.LogInformation("Injury reason with name {InjuryReasonName} created successfully.", command.Name);


        return injuryReason.ToDto();
    }
}