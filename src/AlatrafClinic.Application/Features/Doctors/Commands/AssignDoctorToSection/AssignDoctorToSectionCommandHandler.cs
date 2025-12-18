using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.AssignDoctorToSection;

public class AssignDoctorToSectionCommandHandler(
    IAppDbContext _context,
    ILogger<AssignDoctorToSectionCommandHandler> _logger
) : IRequestHandler<AssignDoctorToSectionCommand, Result<Updated>>
{
   
    public async Task<Result<Updated>> Handle(AssignDoctorToSectionCommand command, CancellationToken ct)
    {
    
        var doctor = await _context.Doctors.FirstOrDefaultAsync(d=> d.Id == command.DoctorId, ct);
        if (doctor is null)
        {
            _logger.LogError("Doctor {DoctorId} not found.", command.DoctorId);
            return ApplicationErrors.DoctorNotFound;
        }

        var section = await _context.Sections.FirstOrDefaultAsync(s=> s.Id == command.SectionId, ct);
        
        if (section is null)
        {
            _logger.LogError("Section {section} not found.", command.SectionId);
            return SectionErrors.SectionNotFound;
        }

        
        var assignResult = doctor.AssignToSection(section, command.Notes);
        if (assignResult.IsError)
        {
            _logger.LogError(
            "Failed to assign Doctor {DoctorId} to new Section {SectionId}: {Error}",
            doctor.Id, section.Id, assignResult.Errors);
        return assignResult.Errors;
        }

        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Doctor {DoctorId} assigned to new Section {SectionId}.",
            doctor.Id, section.Id);

        return Result.Updated;
    }
}