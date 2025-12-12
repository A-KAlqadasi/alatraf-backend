using AlatrafClinic.Api.Requests.Appointments;
using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Appointments.Commands.ChangeAppointmentStatus;
using AlatrafClinic.Application.Features.Appointments.Commands.RescheduleAppointment;
using AlatrafClinic.Application.Features.Appointments.Commands.ScheduleAppointment;
using AlatrafClinic.Application.Features.Appointments.Dtos;
using AlatrafClinic.Application.Features.Appointments.Queries.GetAppointmentById;
using AlatrafClinic.Application.Features.Appointments.Queries.GetAppointments;
using AlatrafClinic.Application.Features.Appointments.Queries.GetNextValidAppointmentDate;
using AlatrafClinic.Domain.Services.Enums;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/appointments")]
[ApiVersion("1.0")]
public sealed class AppointmentsController(ISender sender) : ApiController
{
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<AppointmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of appointments.")]
    [EndpointDescription("Supports filtering appointments by search term, patient type, appointment status, and attend date range. Sorting is customizable.")]
    [EndpointName("GetAppointments")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Get([FromQuery] AppointmentsFilterRequest filters, [FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        if (pageRequest.Page <= 0)
        {
            return BadRequest("Page must be greater than 0");
        }

        if (pageRequest.PageSize <= 0 || pageRequest.PageSize > 100)
        {
            return BadRequest("PageSize must be between 1 and 100");
        }

        var query = new GetAppointmentsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filters.SearchTerm,
            filters.Status is not null ? (AppointmentStatus)(int)filters.Status : null,
            filters.PatientType,
            filters.FromDate,
            filters.ToDate,
            filters.SortColumn,
            filters.SortDirection
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem);
    }

    [HttpGet("{appointmentId:int}", Name = "GetAppointmentById")]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves an appointment by its ID.")]
    [EndpointDescription("Returns detailed information about the specified appointment if it exists.")]
    [EndpointName("GetAppointmentById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(int appointmentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetAppointmentByIdQuery(appointmentId), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new appointment.")]
    [EndpointDescription("Creates a new appointment, specifying ticket, requested date, and notes if any")]
    [EndpointName("CreateAppointment")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] ScheduleAppointmentRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new ScheduleAppointmentCommand(request.TicketId, request.RequestedDate, request.Notes), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetAppointmentById",
                routeValues: new { version = "1.0", appointmentId = response.Id },
                value: response),
                Problem
        );
    }

    [HttpGet("last-valid-appointment-date")]
    [ProducesResponseType(typeof(DateOnly), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Get Last Valid Appointment Date")]
    [EndpointDescription("Get last valid appointment date based on requested date if provided")]
    [EndpointName("GetLastValidAppointmentDate")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetLastValidAppointmentDate([FromQuery] DateOnly? requestedDate, CancellationToken ct)
    {
        var result = await sender.Send(new GetNextValidAppointmentDateQuery(requestedDate), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPatch("{appointmentId:int}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update a appointment status.")]
    [EndpointDescription("Updates an existing appointment with the provided details.")]
    [EndpointName("UpdateAppointmentStatus")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> UpdateStatus(int appointmentId, [FromBody] ChangeAppointmentStatusRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new ChangeAppointmentStatusCommand(appointmentId, request.Status), ct);
      
         return result.Match(
            response => NoContent(),
                Problem
        );
    }

    [HttpPatch("{appointmentId:int}/reschedule")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Change appointment date.")]
    [EndpointDescription("Updates appointment date to a new valid date.")]
    [EndpointName("UpdateAppointmentDate")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> UpdateDate(int appointmentId, [FromBody] RescheduleAppointmentRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new RescheduleAppointmentCommand(appointmentId, request.NewAttendDate), ct);
      
         return result.Match(
            response => NoContent(),
                Problem
        );
    }
    
}