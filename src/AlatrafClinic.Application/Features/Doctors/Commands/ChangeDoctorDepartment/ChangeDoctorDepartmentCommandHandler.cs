using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.ChangeDoctorDepartment;

public class ChangeDoctorDepartmentCommandHandler(
    IAppDbContext _context,
    ILogger<ChangeDoctorDepartmentCommandHandler> _logger) : IRequestHandler<ChangeDoctorDepartmentCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(ChangeDoctorDepartmentCommand command, CancellationToken ct)
    {
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d=> d.Id == command.DoctorId, ct);
        if (doctor is null)
        {
            _logger.LogError("Doctor {DoctorId} not found.", command.DoctorId);
            return ApplicationErrors.DoctorNotFound;
        }

        var department = await _context.Departments.FirstOrDefaultAsync(d=> d.Id == command.NewDepartmentId, ct);
        if (department is null)
        {
            _logger.LogError("Department {DepartmentId} not found.", command.NewDepartmentId);
            return ApplicationErrors.DepartmentNotFound;
        }

        var changeResult = doctor.ChangeDepartment(command.NewDepartmentId);
        if (changeResult.IsError)
        {
            _logger.LogError("Failed to change department for Doctor {DoctorId}: {Error}", doctor.Id, changeResult.Errors);
            return changeResult.Errors;
        }

        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync(ct);
        
        _logger.LogInformation("Doctor {DoctorId} transferred to Department {DepartmentId}.", doctor.Id, department.Id);

        return Result.Updated;
    }
}