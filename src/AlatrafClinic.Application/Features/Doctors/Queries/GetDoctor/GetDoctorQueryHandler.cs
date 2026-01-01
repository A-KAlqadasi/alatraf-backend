using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.People.Doctors;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Doctors.Queries.GetDoctor;

public class GetDoctorQueryHandler : IRequestHandler<GetDoctorQuery, Result<DoctorDto>>
{
    private readonly ILogger<GetDoctorQueryHandler> _logger;
    private readonly IAppDbContext _context;

    public GetDoctorQueryHandler(ILogger<GetDoctorQueryHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<Result<DoctorDto>> Handle(GetDoctorQuery query, CancellationToken ct)
    {
        var doctor = await _context.Doctors
        .Include(d=> d.Person)
        .Include(d=> d.Assignments)
        .FirstOrDefaultAsync(d=> d.Id == query.DoctorId);

        if(doctor is null)
        {
            _logger.LogError("Doctor with Id {doctorId} is not found", query.DoctorId);
            return DoctorErrors.NotFound;
        }

        return doctor.ToDto();
    }
}