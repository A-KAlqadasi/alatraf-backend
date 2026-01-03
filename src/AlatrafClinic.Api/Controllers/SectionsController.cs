using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Sections;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Queries.GetDoctorsBySectionRoom;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Queries.GetRoomsBySectionId;
using AlatrafClinic.Application.Features.Sections.Commands.AssignNewRoomsToSection;
using AlatrafClinic.Application.Features.Sections.Commands.CreateSection;
using AlatrafClinic.Application.Features.Sections.Commands.DeleteSection;
using AlatrafClinic.Application.Features.Sections.Commands.UpdateSection;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Queries.GetSectionById;
using AlatrafClinic.Application.Features.Sections.Queries.GetSections;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/sections")]
[ApiVersion("1.0")]
public sealed class SectionsController(ISender sender) : ApiController
{
    [HttpGet("{sectionId:int}/rooms", Name = "GetSectionRooms")]
    [ProducesResponseType(typeof(List<SectionRoomDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves section rooms.")]
    [EndpointDescription("Returns a list of rooms associated with the specified section.")]
    [EndpointName("GetSectionRooms")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetRoomsBySectionId(int sectionId, CancellationToken ct)
    {
        var result = await sender.Send(new GetRoomsBySectionIdQuery(sectionId), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpGet("{sectionId:int}/rooms/{roomId:int}/doctors", Name = "GetDoctorsBySectionRoom")]
    [ProducesResponseType(typeof(List<GetDoctorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves doctors in section-room by section id and room id.")]
    [EndpointDescription("Returns a list of doctors associated with the specified section and room.")]
    [EndpointName("GetDoctorsBySectionRoom")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetDoctorsBySectionRoom(int sectionId, int roomId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDoctorsBySectionRoomQuery(sectionId, roomId), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }
    [HttpGet("{sectionId:int}/doctors", Name = "GetDoctorsBySection")]
    [ProducesResponseType(typeof(List<GetDoctorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves doctors in section by sectionId.")]
    [EndpointDescription("Returns a list of doctors associated with the specified section.")]
    [EndpointName("GetDoctorsBySection")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetDoctorsBySection(int sectionId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDoctorsBySectionRoomQuery(sectionId, null), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<SectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of sections.")]
    [EndpointDescription("Supports filtering sections by search term. Sorting is by name.")]
    [EndpointName("GetSections")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get([FromQuery] SectionFilterRequest request, [FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        var result = await sender.Send(new GetSectionsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            request.DepartmentId,
            request.SearchTerm,
            request.SortColumn,
            request.SortDirection
         ), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{sectionId:int}", Name = "GetSectionById")]
    [ProducesResponseType(typeof(SectionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve section by Id.")]
    [EndpointDescription("Retrive section by its Id")]
    [EndpointName("GetSectionById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int sectionId, CancellationToken ct)
    {
        var result = await sender.Send(new GetSectionByIdQuery(sectionId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(SectionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new section.")]
    [EndpointDescription("Creates a new section with the specified details.")]
    [EndpointName("CreateSection")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateSectionRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateSectionCommand(request.DepartmentId, request.Name), ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetSectionById",
                routeValues: new { version = "1.0", sectionId = response.Id },
                value: response),
                Problem
        );
    }

    [HttpPut("{sectionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing section.")]
    [EndpointDescription("Updates an existing section with the specified details.")]
    [EndpointName("UpdateSection")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int sectionId, [FromBody] UpdateSectionRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateSectionCommand(sectionId, request.Name), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{sectionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing section.")]
    [EndpointDescription("Deletes an existing section by its Id.")]
    [EndpointName("DeleteSection")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int sectionId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteSectionCommand(sectionId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
    
    [HttpPost("{sectionId:int}/rooms")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Assigns new rooms to a section.")]
    [EndpointDescription("Assigns new rooms to a section by its Id.")]
    [EndpointName("AssignNewRoomsToSection")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AssignNewRooms(int sectionId, [FromBody] AssignNewRoomsToSectionRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new AssignNewRoomsToSectionCommand(sectionId, request.RoomNames), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}