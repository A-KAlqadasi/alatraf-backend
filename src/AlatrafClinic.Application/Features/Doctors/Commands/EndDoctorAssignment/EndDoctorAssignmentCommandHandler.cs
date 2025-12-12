using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People.Doctors;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.EndDoctorAssignment;

public class EndDoctorAssignmentCommandHandler(
    IAppDbContext _context,
    ILogger<EndDoctorAssignmentCommandHandler> _logger,
    HybridCache _cache
) : IRequestHandler<EndDoctorAssignmentCommand, Result<Updated>>
{
    public async Task<Result<Updated>> Handle(EndDoctorAssignmentCommand command, CancellationToken ct)
    {
        var doctor = await _context.Doctors.Include(d=> d.Assignments).FirstOrDefaultAsync(d=> d.Id == command.DoctorId, ct);
        if (doctor is null)
        {
            _logger.LogWarning("Doctor {DoctorId} not found.", command.DoctorId);
            return ApplicationErrors.DoctorNotFound;
        }

        var activeAssignment = doctor.GetCurrentAssignment();
        if (activeAssignment is null)
        {
            _logger.LogWarning("Doctor {DoctorId} has no active assignment to end.", command.DoctorId);
            return DoctorErrors.NoActiveAssignment;
        }

        var endResult = activeAssignment.EndAssignment();
        if (endResult.IsError)
        {
            _logger.LogWarning(
                "Failed to end assignment for Doctor {DoctorId}: {Error}",
                doctor.Id, endResult.Errors);
            return endResult.Errors;
        }

        await _context.SaveChangesAsync(ct);
        await _cache.RemoveByTagAsync("doctor", ct);

        _logger.LogInformation("Doctor {DoctorId}'s current assignment ended successfully.", doctor.Id);

        return Result.Updated;
    }
}