using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Tickets.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Appointments.Queries.GetAppointments;

public sealed class GetAppointmentsQueryHandler
    : IRequestHandler<GetAppointmentsQuery, Result<PaginatedList<AppointmentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAppointmentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PaginatedList<AppointmentDto>>> Handle(GetAppointmentsQuery query, CancellationToken ct)
    {
         var spec = new AppointmentsFilter(query);

        var page = spec.Page;
        var pageSize = spec.PageSize;

        var totalCount = await _unitOfWork.Appointments.CountAsync(spec, ct);

        var entities = await _unitOfWork.Appointments.ListAsync(spec, page, pageSize, ct);

        var items = entities.Select(a => new AppointmentDto
        {
            Id = a.Id,
            TicketId = a.TicketId,
            Ticket = a.Ticket != null ? a.Ticket.ToDto() : null,
            PatientType = a.PatientType,
            AttendDate = a.AttendDate,
            CreatedAt = a.CreatedAtUtc.DateTime,
            Status = a.Status,
            Notes = a.Notes,
            IsEditable = a.IsEditable,
            IsAppointmentTomorrow = a.IsAppointmentTomorrow()
        }).ToList();

        return new PaginatedList<AppointmentDto>
        {
            Items = items,
            PageNumber = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        };
    }
}