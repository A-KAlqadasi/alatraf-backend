using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.IndustrialParts;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.IndustrialParts.Commands.CreateIndustrialPart;
using AlatrafClinic.Application.Features.IndustrialParts.Commands.DeleteIndustrialPart;
using AlatrafClinic.Application.Features.IndustrialParts.Commands.UpdateIndustrialPart;
using AlatrafClinic.Application.Features.IndustrialParts.Dtos;
using AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartById;
using AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialParts;
using AlatrafClinic.Application.Features.IndustrialParts.Queries.GetIndustrialPartsWithFilters;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/industrial-parts")]
[ApiVersion("1.0")]
public sealed class IndustrialPartsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<IndustrialPartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of industrial parts.")]
    [EndpointDescription("Supports filtering industrial parts by search term. Sorting is by name.")]
    [EndpointName("GetIndustrialParts")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        CancellationToken ct = default)
    {
        var query = new GetIndustrialPartsQuery();
        
        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("with-filters")]
    [ProducesResponseType(typeof(PaginatedList<IndustrialPartDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of industrial parts.")]
    [EndpointDescription("Supports pagination and searching by either part name or part ID (when the search term is numeric). Sorting is customizable.")]
    [EndpointName("GetIndustrialPartsWithFilters")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] IndustrialPartFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetIndustrialPartsWithFiltersQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.SortColumn,
            filter.SortDirection
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }


    [HttpGet("{industrialPartId:int}", Name = "GetIndustrialPartById")]
    [ProducesResponseType(typeof(IndustrialPartDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve industrial part by Id.")]
    [EndpointDescription("Retrive industrial part by its Id")]
    [EndpointName("GetIndustrialPartById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int industrialPartId, CancellationToken ct)
    {
        var result = await sender.Send(new GetIndustrialPartByIdQuery(industrialPartId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(IndustrialPartDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new industrial part.")]
    [EndpointDescription("Creates a new industrial part with the specified details.")]
    [EndpointName("CreateIndustrialPart")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateIndustrialPartRequest request, CancellationToken ct)
    {
        var units = request.Units.ConvertAll(u=> new CreateIndustrialPartUnitCommand(u.UnitId, u.Price));

        var result = await sender.Send(new CreateIndustrialPartCommand(request.Name, request.Description, units), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetIndustrialPartById",
                routeValues: new { version = "1.0", industrialPartId = response.IndustrialPartId },
                value: response),
                Problem
        );
    }

    [HttpPut("{industrialPartId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing industrial part.")]
    [EndpointDescription("Updates an existing industrial part with the specified details.")]
    [EndpointName("UpdateIndustrialPart")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int industrialPartId, [FromBody] UpdateIndustrialPartRequest request, CancellationToken ct)
    {
        var units = request.Units.ConvertAll(u=> new UpdateIndustrialPartUnitCommand(u.UnitId, u.Price));

        var result = await sender.Send(new UpdateIndustrialPartCommand( industrialPartId, request.Name, request.Description, units), ct);

        return result.Match(
            _ => NoContent(),
                Problem
        );
    }
    
    [HttpDelete("{industrialPartId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing industrial part.")]
    [EndpointDescription("Deletes an existing industrial part by its Id.")]
    [EndpointName("DeleteIndustrialPart")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int industrialPartId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteIndustrialPartCommand(industrialPartId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

}