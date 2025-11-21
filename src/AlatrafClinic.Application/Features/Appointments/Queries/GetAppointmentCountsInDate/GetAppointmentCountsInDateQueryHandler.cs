
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Patients.Enums;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentCountsInDate;

public class GetAppointmentCountsInDateQueryHandler : IRequestHandler<GetAppointmentCountsInDateQuery, Result<AppointmentCountsDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAppointmentCountsInDateQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<AppointmentCountsDto>> Handle(GetAppointmentCountsInDateQuery query, CancellationToken ct)
    {
        int totalCount = await _unitOfWork.Appointments.GetAppointmentCountByDate(query.Date, ct);
        int normalCount = await _unitOfWork.Appointments.GetAppointmentCountByDateAndPatientType(query.Date, PatientType.Normal, ct);
        int woundedCount = await _unitOfWork.Appointments.GetAppointmentCountByDateAndPatientType(query.Date, PatientType.Wounded, ct);

        var dto = new AppointmentCountsDto
        {
            Date = query.Date,
            TotalCount = totalCount,
            NormalCount = normalCount,
            WoundedCount = woundedCount
        };

        return dto;
    }
}