
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Departments.Commands.DeleteDepartment;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteDepartmentCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDepartmentCommandHandler(ILogger<DeleteDepartmentCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Deleted>> Handle(DeleteDepartmentCommand command, CancellationToken ct)
    {
        var department = await _unitOfWork.Departments.GetByIdAsync(command.DepartmentId, ct);
        if(department is null)
        {
            _logger.LogError("Department with Id {departmentId} is not found", command.DepartmentId);
            return DepartmentErrors.NotFound;
        }

        await _unitOfWork.Departments.DeleteAsync(department, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Department {departmentId} deleted successfully", command.DepartmentId);
        
        return Result.Deleted;
    }
}