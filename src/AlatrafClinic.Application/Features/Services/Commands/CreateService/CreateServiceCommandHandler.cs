using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Commands.CreateService;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Result<ServiceDto>>
{
    private readonly HybridCache _cache;
    private readonly ILogger<CreateServiceCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public CreateServiceCommandHandler(ILogger<CreateServiceCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _cache = cache;
        _logger = logger;
        _context = context;
    }

    public async Task<Result<ServiceDto>> Handle(CreateServiceCommand command, CancellationToken ct)
    {
        Department? department = null;
        if (command.DepartmentId.HasValue)
        {
            department = await _context.Departments.FirstOrDefaultAsync(d=> d.Id == command.DepartmentId, ct);

            if (department is null)
            {
                _logger.LogError("Department with ID {DepartmentId} was not found.", command.DepartmentId);

                return Error.NotFound(code: "Department.NotFound", description: $"Department with ID {command.DepartmentId} was not found.");
            }
        }
        
        var serviceResult = Service.Create(command.Name, command.DepartmentId, command.Price);
        if (serviceResult.IsError)
        {
            _logger.LogError("Failed to create service: {Errors}", serviceResult.Errors);

            return serviceResult.Errors;
        }
        
        var service = serviceResult.Value;
        service.Department = department;

        await _context.Services.AddAsync(service, ct);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("service", ct);
        
        _logger.LogInformation("Service with ID {ServiceId} created successfully.", service.Id);

        return service.ToDto();
    }
}