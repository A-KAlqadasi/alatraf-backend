
using AlatrafClinic.Api.Requests.Injuries;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.InjurTypes.Commands.CreateInjuryType;
using AlatrafClinic.Application.Features.InjurTypes.Commands.DeleteInjuryType;
using AlatrafClinic.Application.Features.InjurTypes.Commands.UpdateInjuryType;
using AlatrafClinic.Application.Features.InjurTypes.Queries.GetInjuryTypeById;
using AlatrafClinic.Application.Features.InjurTypes.Queries.GetInjuryTypes;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/injury-types")]
[ApiVersion("1.0")]
public sealed class InjuryTypesController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<InjuryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve injury types.")]
    [EndpointDescription("Retrive all the injury types in the system.")]
    [EndpointName("GetInjuryTypes")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetInjuryTypesQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{injuryTypeId:int}", Name = "GetInjuryTypeById")]
    [ProducesResponseType(typeof(InjuryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve injury type by Id.")]
    [EndpointDescription("Retrive injury type by its Id")]
    [EndpointName("GetInjuryTypeById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int injuryTypeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetInjuryTypeByIdQuery(injuryTypeId), ct);
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
    [EndpointSummary("Creates a new injury type.")]
    [EndpointDescription("Creates a new injury type with the specified details.")]
    [EndpointName("CreateInjuryType")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateInjuryRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateInjuryTypeCommand(request.Name), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetInjuryTypeById",
                routeValues: new { version = "1.0", injuryTypeId = response.Id },
                value: response),
                Problem
        );
    }

    [HttpPut("{injuryTypeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing injury type.")]
    [EndpointDescription("Updates an existing injury type with the specified details.")]
    [EndpointName("UpdateInjuryType")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int injuryTypeId, [FromBody] UpdateInjuryRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateInjuryTypeCommand(injuryTypeId, request.Name), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{injuryTypeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing injury type.")]
    [EndpointDescription("Deletes an existing injury type with the specified details.")]
    [EndpointName("DeleteInjuryType")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int injuryTypeId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteInjuryTypeCommand(injuryTypeId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
    

}