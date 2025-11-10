using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Application.Features.Services.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Services;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Services.Commands.CreateService;

public class CreateServiceCommandHandler : IRequestHandler<CreateServiceCommand, Result<ServiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;
    private readonly ILogger<CreateServiceCommandHandler> _logger;

    public CreateServiceCommandHandler(ILogger<CreateServiceCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache)
    {
        _unitOfWork = unitOfWork;
        _cache = cache;
        _logger = logger;
    }

    public async Task<Result<ServiceDto>> Handle(CreateServiceCommand command, CancellationToken ct)
    {

        var department = await _unitOfWork.Departments.GetByIdAsync(command.DepartmentId, ct);

        if (department is null)
        {
            _logger.LogWarning("Department with ID {DepartmentId} was not found.", command.DepartmentId);
            return Error.NotFound(code: "Department.NotFound", description: $"Department with ID {command.DepartmentId} was not found.");
        }
        
        var serviceResult = Service.Create(command.Name, command.DepartmentId, command.Price);
        if (serviceResult.IsError)
        {
            _logger.LogWarning("Failed to create service: {Errors}", serviceResult.Errors);
            return serviceResult.Errors;
        }
        var service = serviceResult.Value;
        service.Department = department;
        await _unitOfWork.Services.AddAsync(service, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        _logger.LogInformation("Service with ID {ServiceId} created successfully.", service.Id);

        return service.ToDto();
    }
}