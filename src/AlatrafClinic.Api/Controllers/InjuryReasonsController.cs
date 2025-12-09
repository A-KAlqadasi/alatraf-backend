
using AlatrafClinic.Api.Requests.Injuries;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.InjuryReasons.Commands.DeleteInjuryReason;
using AlatrafClinic.Application.Features.InjuryReasons.Commands.UpdateInjuryReason;
using AlatrafClinic.Application.Features.InjuryReasons.Queries.GetInjuryReasonById;
using AlatrafClinic.Application.Features.InjuryReasons.Queries.GetInjuryReasons;
using AlatrafClinic.Application.Features.InjuryReasons.Commands.CreateInjuryReason;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/injury-reasons")]
[ApiVersion("1.0")]
public sealed class InjuryReasonsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<InjuryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve injury reasons.")]
    [EndpointDescription("Retrive all the injury reasons in the system.")]
    [EndpointName("GetInjuryReasons")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetInjuryReasonsQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{injuryReasonId:int}", Name = "GetInjuryReasonById")]
    [ProducesResponseType(typeof(InjuryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve injury reason by Id.")]
    [EndpointDescription("Retrive injury reason by its Id")]
    [EndpointName("GetInjuryReasonById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int injuryReasonId, CancellationToken ct)
    {
        var result = await sender.Send(new GetInjuryReasonByIdQuery(injuryReasonId), ct);
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
    [EndpointSummary("Creates a new injury reason.")]
    [EndpointDescription("Creates a new injury reason with the specified details.")]
    [EndpointName("CreateInjuryReason")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateInjuryRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateInjuryReasonCommand(request.Name), ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetInjuryReasonById",
                routeValues: new { version = "1.0", InjuryReasonId = response.Id },
                value: response),
                Problem
        );
    }

    [HttpPut("{injuryReasonId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing injury reason.")]
    [EndpointDescription("Updates an existing injury reason with the specified details.")]
    [EndpointName("UpdateInjuryReason")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int injuryReasonId, [FromBody] UpdateInjuryRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateInjuryReasonCommand(injuryReasonId, request.Name), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{injuryReasonId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing injury reason.")]
    [EndpointDescription("Deletes an existing injury reason with the specified details.")]
    [EndpointName("DeleteInjuryReason")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int injuryReasonId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteInjuryReasonCommand(injuryReasonId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
    

}