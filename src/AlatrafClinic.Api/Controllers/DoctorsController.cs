using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Doctors;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Commands.AssignDoctorToSectionAndRoom;
using AlatrafClinic.Application.Features.Doctors.Commands.CreateDoctor;
using AlatrafClinic.Application.Features.Doctors.Commands.EndDoctorAssignment;
using AlatrafClinic.Application.Features.Doctors.Commands.UpdateDoctor;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Queries.GetDoctor;
using AlatrafClinic.Application.Features.Doctors.Queries.GetDoctors;
using AlatrafClinic.Application.Features.Doctors.Queries.GetTechniciansDropdown;
using AlatrafClinic.Application.Features.Doctors.Queries.GetTherapistDropdown;
using AlatrafClinic.Application.Features.People.Doctors.Commands.AssignDoctorToRoom;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;


[Route("api/v{version:apiVersion}/doctors")]
[ApiVersion("1.0")]
public sealed class DoctorsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<DoctorListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of doctors.")]
    [EndpointDescription("Supports filtering doctors by various criteria including search term, roomId , sectionId and HasActiveAssignments, sort is customizeable.")]
    [EndpointName("GetDoctors")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] DoctorsFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetDoctorsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.DepartmentId,
            filter.AddressId,
            filter.SectionId,
            filter.RoomId,
            filter.Search,
            filter.Specialization,
            filter.HasActiveAssignment,
            filter.SortBy,
            filter.SortDir
        );

        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }
    [HttpGet("{doctorId:int}", Name = "GetDoctorById")]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a doctor by its ID.")]
    [EndpointDescription("Fetches detailed information about a specific doctor using its unique identifier.")]
    [EndpointName("GetDoctorById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int doctorId, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetDoctorQuery(doctorId), ct);
        
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(DoctorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new doctor.")]
    [EndpointDescription("Creates a new doctor with the provided details and returns the created patient information.")]
    [EndpointName("CreateDoctor")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateDoctorRequest request, CancellationToken ct = default)
    {

        var result = await sender.Send(new CreateDoctorCommand(
            request.Fullname,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.AddressId,
            request.Gender,
            request.Specialization,
            request.DepartmentId
        ), ct);

         return result.Match(
            response => CreatedAtRoute(
                routeName: "GetDoctorById",
                routeValues: new { version = "1.0", doctorId = response.DoctorId },
                value: response),
                Problem
        );
    }

    [HttpPut("{doctorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update existing doctor.")]
    [EndpointDescription("Update a doctor with the new provided details.")]
    [EndpointName("UpdateDoctor")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(int doctorId, [FromBody] UpdateDoctortRequest request, CancellationToken ct = default)
    {

        var result = await sender.Send(new UpdateDoctorCommand(
            doctorId,
            request.Fullname,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.AddressId,
            request.Gender,
            request.Specialization,
            request.DepartmentId
        ), ct);

         return result.Match(
            _=> NoContent()
            , Problem);
    }
    
    [HttpPatch("end-assignment/{doctorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("End Doctor Assignement.")]
    [EndpointDescription("End the specified doctor assignement.")]
    [EndpointName("EndDoctorAssignment")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> EndDoctorAssignment(int doctorId, CancellationToken ct)
    {
        var result = await sender.Send(new EndDoctorAssignmentCommand(doctorId), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpPut("assign-to-section/{doctorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Assign Doctor to section.")]
    [EndpointDescription("Assign doctor to section.")]
    [EndpointName("AssignDoctorToSection")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> AssignDoctorToSection(int doctorId, [FromBody] AssignDoctorToSectionRequest request, CancellationToken ct = default)
    {

        var result = await sender.Send(new AssignDoctorToSectionCommand(
            doctorId,
            request.SectionId,
            request.Notes
        ), ct);

         return result.Match(
            _=> NoContent()
            , Problem);
    }

    [HttpPut("assign-to-section-room/{doctorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Assign Doctor to section and room.")]
    [EndpointDescription("Assign doctor to section and room or only to section.")]
    [EndpointName("AssignDoctorToSectionRoom")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> AssignDoctorToSectionRoom(int doctorId, [FromBody] AssignDoctorToSectionRoomRequest request, CancellationToken ct = default)
    {

        var result = await sender.Send(new AssignDoctorToSectionAndRoomCommand(
            doctorId,
            request.SectionId,
            request.IsActive,
            request.RoomId,
            request.Notes
        ), ct);

         return result.Match(
            _=> NoContent()
            , Problem);
    }

    [HttpGet("technicians")]
    [ProducesResponseType(typeof(PaginatedList<TechnicianDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves technicians for dropdown selection.")]
    [EndpointDescription(
        "Returns a paginated list of technicians filtered by section and optional search term. " +
        "Designed for dropdown and autocomplete scenarios."
    )]
    [EndpointName("GetTechniciansDropdown")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetTechniciansDropdown(
        [FromQuery] TechnicianFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetTechniciansDropdownQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SectionId,
            filter.SearchTerm
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("therapists")]
    [ProducesResponseType(typeof(PaginatedList<TherapistDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves therapists for dropdown selection.")]
    [EndpointDescription(
        "Returns a paginated list of therapists filtered by section and optional search term. " +
        "Designed for dropdown and autocomplete scenarios."
    )]
    [EndpointName("GetTherapistsDropdown")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetTherapistsDropdown(
        [FromQuery] TherapistFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetTherapistDropdownQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SectionId,
            filter.RoomId,
            filter.SearchTerm
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }
    
}