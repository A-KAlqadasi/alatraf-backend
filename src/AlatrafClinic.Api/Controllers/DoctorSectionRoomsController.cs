using AlatrafClinic.Api.Requests.DoctorSectionRooms;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedHeader;
using AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianIndustrialParts;
using AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistAssignedHeader;
using AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistSessions;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/doctor-section-rooms")]
[ApiVersion("1.0")]
public sealed class DoctorSectionRoomsController(ISender sender) : ApiController
{
    // 1. Action for GetTechnicianAssignedHeaderQuery
    [HttpGet("{doctorSectionRoomId:int}/technician-header", Name = "GetTechnicianHeader")]
    [ProducesResponseType(typeof(TechnicianHeaderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves the technician's header information.")]
    [EndpointDescription("Returns header details like technician name and assigned room.")]
    [EndpointName("GetTechnicianHeader")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetTechnicianHeader(int doctorSectionRoomId, CancellationToken ct)
    {
        var result = await sender.Send(new GetTechnicianAssignedHeaderQuery(doctorSectionRoomId), ct);
        return result.Match(
            response => Ok(response),
            Problem);
    }

    // 2. Action for GetTechnicianIndustrialPartsQuery
    [HttpGet("{doctorSectionRoomId:int}/technician-industrial-parts", Name = "GetTechnicianIndustrialParts")]
    [ProducesResponseType(typeof(PaginatedList<TechnicianIndustrialPartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves technician industrial parts.")]
    [EndpointDescription("Returns a paginated list of industrial parts assigned to a technician, with optional filters.")]
    [EndpointName("GetTechnicianIndustrialParts")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetTechnicianIndustrialParts(
        int doctorSectionRoomId,
        [FromQuery] GetTechnicianIndustrialPartsFilter filter,
        CancellationToken ct)
    {
        var query = new GetTechnicianIndustrialPartsQuery(
            doctorSectionRoomId,
            filter.date,
            filter.repairCardId,
            filter.patientName,
            filter.Page,
            filter.PageSize);
            
        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem);
    }

    // 3. Action for GetTherapistAssignedHeaderQuery
    [HttpGet("{doctorSectionRoomId:int}/therapist-header", Name = "GetTherapistHeader")]
    [ProducesResponseType(typeof(TherapistHeaderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves the therapist's header information.")]
    [EndpointDescription("Returns header details like therapist name and assigned room.")]
    [EndpointName("GetTherapistHeader")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetTherapistHeader(int doctorSectionRoomId, CancellationToken ct)
    {
        var result = await sender.Send(new GetTherapistAssignedHeaderQuery(doctorSectionRoomId), ct);
        return result.Match(
            response => Ok(response),
            Problem);
    }

    // 4. Action for GetTherapistSessionsQuery
    [HttpGet("{doctorSectionRoomId:int}/therapist-sessions", Name = "GetTherapistSessions")]
    [ProducesResponseType(typeof(PaginatedList<TherapistSessionProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves therapist sessions.")]
    [EndpointDescription("Returns a paginated list of therapy sessions, with optional filters for date, patient, and therapy card.")]
    [EndpointName("GetTherapistSessions")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetTherapistSessions(
        int doctorSectionRoomId,
        [FromQuery] GetTherapistSessionsFilter filter,
        CancellationToken ct)
    {
        var query = new GetTherapistSessionsQuery(
            doctorSectionRoomId,
            filter.Date,
            filter.PatientName,
            filter.TherapyCardId,
            filter.Page,
            filter.PageSize);

        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem);
    }

}