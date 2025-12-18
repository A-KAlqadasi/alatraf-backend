
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Application.Features.Departments.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Organization.Departments.Commands.CreateDepartment;

public sealed class CreateDepartmentCommandHandler(
    IAppDbContext context,
    ILogger<CreateDepartmentCommandHandler> logger,

    HybridCache cache
) : IRequestHandler<CreateDepartmentCommand, Result<DepartmentDto>>
{
    private readonly IAppDbContext _context = context;
    private readonly ILogger<CreateDepartmentCommandHandler> _logger = logger;
    private readonly HybridCache _cache = cache;
    public async Task<Result<DepartmentDto>> Handle(CreateDepartmentCommand command, CancellationToken ct)
    {
        var name = command.Name.Trim();

        var existing = await _context.Departments.AnyAsync(d=> d.Name == name, ct);
        if (existing)
        {
            _logger.LogWarning("Department with name '{DepartmentName}' already exists.", name);
            return ApplicationErrors.DepartmentAlreadyExists(name);
        }

        var departmentResult = Department.Create(name);
        if (departmentResult.IsError)
        return departmentResult.Errors;

        var department = departmentResult.Value;

        await _context.Departments.AddAsync(department, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(" Department '{DepartmentName}' created successfully with ID {DepartmentId}.", name, department.Id);

        return department.ToDto();
    }
}