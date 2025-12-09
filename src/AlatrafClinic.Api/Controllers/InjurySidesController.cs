
using AlatrafClinic.Api.Requests.Injuries;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.InjurySides.Commands.CreateInjurySide;
using AlatrafClinic.Application.Features.InjurySides.Commands.DeleteInjurySide;
using AlatrafClinic.Application.Features.InjurySides.Commands.UpdateInjurySide;
using AlatrafClinic.Application.Features.InjurySides.Queries.GetInjurySideById;
using AlatrafClinic.Application.Features.InjurySides.Queries.GetInjurySides;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/injury-sides")]
[ApiVersion("1.0")]
public sealed class InjurySidesController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<InjuryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve injury sides.")]
    [EndpointDescription("Retrive all the injury sides in the system.")]
    [EndpointName("GetInjurySides")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetInjurySidesQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{injurySideId:int}", Name = "GetInjurySideById")]
    [ProducesResponseType(typeof(InjuryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve injury side by Id.")]
    [EndpointDescription("Retrive injury type by its Id")]
    [EndpointName("GetInjurySideById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int injurySideId, CancellationToken ct)
    {
        var result = await sender.Send(new GetInjurySideByIdQuery(injurySideId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(InjuryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new injury side.")]
    [EndpointDescription("Creates a new injury side with the specified details.")]
    [EndpointName("CreateInjurySide")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateInjuryRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateInjurySideCommand(request.Name), ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetInjurySideById",
                routeValues: new { version = "1.0", injurySideId = response.Id },
                value: response),
                Problem
        );
    }

    [HttpPut("{injurySideId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing injury side.")]
    [EndpointDescription("Updates an existing injury side with the specified details.")]
    [EndpointName("UpdateInjurySide")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int injurySideId, [FromBody] UpdateInjuryRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateInjurySideCommand(injurySideId, request.Name), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{injurySideId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing injury side.")]
    [EndpointDescription("Deletes an existing injury side with the specified details.")]
    [EndpointName("DeleteInjurySide")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int injurySideId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteInjurySideCommand(injurySideId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
    

}