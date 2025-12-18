
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Patients.Commands.DeletePatient;

public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Result<Deleted>>
{
    private readonly ILogger<DeleteItemCommandHandler> _logger;
    private readonly IAppDbContext _context;

    public DeletePatientCommandHandler(ILogger<DeleteItemCommandHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<Deleted>> Handle(DeletePatientCommand command, CancellationToken ct)
    {
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Id == command.PatientId, ct);

        if(patient is null)
        {
            _logger.LogError("Patient with Id {paitentid} is not found", command.PatientId);
            return PatientErrors.PatientNotFound;
        }

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Patient with Id {paitentid} is deleted successfully", command.PatientId);

        return Result.Deleted;
    }
}