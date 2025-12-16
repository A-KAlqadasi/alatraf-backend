using AlatrafClinic.Application.Features.Doctors.Dtos;
using AlatrafClinic.Application.Features.Doctors.Queries.GetTechnicianAssignedIndustrialParts;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/doctor-section-rooms")]
[ApiVersion("1.0")]
public sealed class DoctorSectionRoomsController(ISender sender) : ApiController
{
    [HttpGet("{doctorSectionRoomId:int}/industrial-parts",
    Name = "GetTechnicianAssignedIndustrialParts")]
    [ProducesResponseType(typeof(List<TechnicianIndustrialPartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves technician assigned industrial parts.")]
    [EndpointDescription(
        "Returns the list of industrial parts assigned to the technician for the specified doctor section room.")]
    [EndpointName("GetTechnicianAssignedIndustrialParts")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetTechnicianAssignedIndustrialParts(
        int doctorSectionRoomId,
        CancellationToken ct)
    {
        var result = await sender.Send(
            new GetTechnicianAssignedIndustrialPartsQuery(doctorSectionRoomId),
            ct);

        return result.Match(
            success => Ok(success),
            Problem);
    }

}