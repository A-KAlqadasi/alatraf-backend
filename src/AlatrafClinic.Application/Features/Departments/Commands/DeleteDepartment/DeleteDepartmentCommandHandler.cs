
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteDepartmentCommandHandler> _logger;
    private readonly IAppDbContext _context;
    private readonly HybridCache _cache;

    public DeleteDepartmentCommandHandler(ILogger<DeleteDepartmentCommandHandler> logger, IAppDbContext context, HybridCache cache)
    {
        _logger = logger;
        _context = context;
        _cache = cache;
    }
    public async Task<Result<Deleted>> Handle(DeleteDepartmentCommand command, CancellationToken ct)
    {
        var department = await _context.Departments.FirstOrDefaultAsync(d=> d.Id == command.DepartmentId, ct);
        if(department is null)
        {
            _logger.LogError("Department with Id {departmentId} is not found", command.DepartmentId);
            return DepartmentErrors.NotFound;
        }

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("department", ct);
        
        _logger.LogInformation("Department {departmentId} deleted successfully", command.DepartmentId);
        
        return Result.Deleted;
    }
}