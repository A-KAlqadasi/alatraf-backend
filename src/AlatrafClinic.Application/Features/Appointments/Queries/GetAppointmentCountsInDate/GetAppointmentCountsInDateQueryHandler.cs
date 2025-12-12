
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentCountsInDate;

public class GetAppointmentCountsInDateQueryHandler : IRequestHandler<GetAppointmentCountsInDateQuery, Result<AppointmentCountsDto>>
{
    private readonly IAppDbContext _context;

    public GetAppointmentCountsInDateQueryHandler(IAppDbContext context)
    {
        _context = context;
    }
    public async Task<Result<AppointmentCountsDto>> Handle(GetAppointmentCountsInDateQuery query, CancellationToken ct)
    {
        var date = query.Date ?? AlatrafClinicConstants.TodayDate;
        int totalCount = await _context.Appointments
        .Where(a => DateOnly.FromDateTime(a.CreatedAtUtc.DateTime) == date).CountAsync(ct);
        
        int normalCount = await _context.Appointments
        .Where(a => DateOnly.FromDateTime(a.CreatedAtUtc.DateTime) == date && a.PatientType == PatientType.Normal)
            .CountAsync(ct);
            
        int woundedCount = await _context.Appointments
        .Where(a => DateOnly.FromDateTime(a.CreatedAtUtc.DateTime) == date && a.PatientType == PatientType.Wounded)
            .CountAsync(ct);

        var dto = new AppointmentCountsDto
        {
            Date = date,
            TotalCount = totalCount,
            NormalCount = normalCount,
            WoundedCount = woundedCount
        };

        return dto;
    }
}