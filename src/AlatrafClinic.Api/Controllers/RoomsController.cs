
using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Rooms;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Rooms.Commands.CreateRoom;
using AlatrafClinic.Application.Features.Rooms.Commands.DeleteRoom;
using AlatrafClinic.Application.Features.Rooms.Commands.UpdateRoom;
using AlatrafClinic.Application.Features.Rooms.Dtos;
using AlatrafClinic.Application.Features.Rooms.Queries.GetRoomById;
using AlatrafClinic.Application.Features.Rooms.Queries.GetRooms;

using Asp.Versioning;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/rooms")]
[ApiVersion("1.0")]
public sealed class RoomsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RoomDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of rooms.")]
    [EndpointDescription("Supports filtering by search term, sectionId, and departmentId. Sorting is customizable.")]
    [EndpointName("GetRooms")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] RoomsFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetRoomsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.SectionId,
            filter.DepartmentId,
            filter.SortColumn,
            filter.SortDirection
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{roomId:int}", Name = "GetRoomById")]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a room by its ID.")]
    [EndpointDescription("Fetches detailed information about a specific room using its unique identifier.")]
    [EndpointName("GetRoomById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int roomId, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetRoomByIdQuery(roomId), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(RoomDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new room.")]
    [EndpointDescription("Creates a new room under a specific section and returns the created room details.")]
    [EndpointName("CreateRoom")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateRoomRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateRoomCommand(
            request.SectionId,
            request.Name
        ), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetRoomById",
                routeValues: new { version = "1.0", roomId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPut("{roomId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing room.")]
    [EndpointDescription("Updates a room name using its unique identifier.")]
    [EndpointName("UpdateRoom")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(int roomId, [FromBody] UpdateRoomRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateRoomCommand(
            roomId,
            request.NewName
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{roomId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing room.")]
    [EndpointDescription("Deletes a room using its unique identifier.")]
    [EndpointName("DeleteRoom")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Delete(int roomId, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteRoomCommand(roomId), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}