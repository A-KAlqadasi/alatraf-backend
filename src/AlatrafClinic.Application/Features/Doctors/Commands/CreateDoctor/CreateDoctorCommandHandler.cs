using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;
using AlatrafClinic.Application.Features.People.Services.CreatePerson;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People.Doctors;

using MechanicShop.Application.Common.Errors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Commands.CreateDoctor;

public class CreateDoctorCommandHandler(
    IPersonCreateService _personCreateService,
    IAppDbContext _context,
    ILogger<CreateDoctorCommandHandler> _logger
    ) : IRequestHandler<CreateDoctorCommand, Result<DoctorDto>>
{

    public async Task<Result<DoctorDto>> Handle(CreateDoctorCommand command, CancellationToken ct)
    {
        var personResult = await _personCreateService.CreateAsync(
            command.Fullname,
            command.Birthdate,
            command.Phone,
            command.NationalNo,
            command.Address,
            command.Gender,
            ct);

        if (personResult.IsError)
        return personResult.Errors;

        var person = personResult.Value;



        var department = await _context.Departments.FirstOrDefaultAsync(d=> d.Id == command.DepartmentId, ct);
        if (department is null)
        {
            _logger.LogWarning("Department with ID {DepartmentId} not found.", command.DepartmentId);
            return ApplicationErrors.DepartmentNotFound;
        }

        var doctorResult = Doctor.Create(
            person.Id,
            command.DepartmentId,
            command.Specialization);

        if (doctorResult.IsError)
        {
            return doctorResult.Errors;
        }

        var doctor = doctorResult.Value;
        person.AssignDoctor(doctor);

        await _context.People.AddAsync(person, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Doctor created successfully with ID: {DoctorId}", doctor.Id);

        return doctor.ToDto();
    }
}