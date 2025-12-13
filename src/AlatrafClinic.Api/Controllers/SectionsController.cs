using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Queries.GetDoctorsBySectionRoom;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Queries.GetRoomsBySectionId;

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
    [EndpointSummary("Retrieves doctors by section and room.")]
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
    [EndpointSummary("Retrieves doctors by section and room.")]
    [EndpointDescription("Returns a list of doctors associated with the specified section and room.")]
    [EndpointName("GetDoctorsBySection")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetDoctorsBySection(int sectionId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDoctorsBySectionRoomQuery(sectionId, null), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }
}