using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AlatrafClinic.Application.Features.Inventory.Units.Dtos;

using AlatrafClinic.Application.Features.Inventory.Units.Commands.CreateUnitCommand;
using AlatrafClinic.Application.Features.Inventory.Units.Commands.UpdateUnitCommand;
using AlatrafClinic.Application.Features.Inventory.Units.Commands.DeleteUnitCommand;

using AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitsListQuery;
using AlatrafClinic.Application.Features.Inventory.Units.Queries.GetUnitByIdQuery;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/units")]
[ApiVersion("1.0")]
public sealed class UnitsController(ISender sender) : ApiController
{
    // Queries
    [HttpGet]
    [ProducesResponseType(typeof(List<UnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve units.")]
    [EndpointDescription("Retrieves all units from the system.")]
    [EndpointName("GetAllUnits")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetUnitsListQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{unitId:int}", Name = "GetUnitById")]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve unit by Id.")]
    [EndpointDescription("Retrieves a unit by its identifier.")]
    [EndpointName("GetUnitById")]
    public async Task<IActionResult> GetById(int unitId, CancellationToken ct)
    {
        var result = await sender.Send(new GetUnitByIdQuery(unitId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    // Commands
    [HttpPost]
    [ProducesResponseType(typeof(UnitDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new unit.")]
    [EndpointDescription("Creates a new unit with the provided details.")]
    [EndpointName("CreateUnit")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateUnitCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetUnitById",
                routeValues: new { version = "1.0", unitId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPut("{unitId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing unit.")]
    [EndpointDescription("Updates an existing unit's details.")]
    [EndpointName("UpdateUnit")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int unitId, [FromBody] UpdateUnitCommand command, CancellationToken ct)
    {
        var cmd = new UpdateUnitCommand(unitId, command.Name);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{unitId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing unit.")]
    [EndpointDescription("Deletes the specified unit.")]
    [EndpointName("DeleteUnit")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int unitId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteUnitCommand(unitId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
