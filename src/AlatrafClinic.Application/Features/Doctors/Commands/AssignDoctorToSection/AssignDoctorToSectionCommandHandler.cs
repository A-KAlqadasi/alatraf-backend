using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Departments.Sections;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.AssignDoctorToSection;

public class AssignDoctorToSectionCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<AssignDoctorToSectionCommandHandler> logger
) : IRequestHandler<AssignDoctorToSectionCommand, Result<Updated>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<AssignDoctorToSectionCommandHandler> _logger = logger;

    public async Task<Result<Updated>> Handle(AssignDoctorToSectionCommand command, CancellationToken cancellationToken)
    {
    
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.DoctorId, cancellationToken);
        if (doctor is null)
        {
        _logger.LogError("Doctor {DoctorId} not found.", command.DoctorId);
        return ApplicationErrors.DoctorNotFound;
        }

        var section = await _unitOfWork.Sections.GetByIdAsync(command.SectionId, cancellationToken);
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

        await _unitOfWork.Doctors.UpdateAsync(doctor, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Doctor {DoctorId} assigned to new Section {SectionId}.",
            doctor.Id, section.Id);

        return Result.Updated;
    }
}