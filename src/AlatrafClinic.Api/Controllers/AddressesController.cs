using AlatrafClinic.Api.Requests.Addresses;
using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Addresses.Commands.CreateAddress;
using AlatrafClinic.Application.Features.Addresses.Commands.UpdateAddress;
using AlatrafClinic.Application.Features.Addresses.Dtos;
using AlatrafClinic.Application.Features.Addresses.Queries.GetAddress;
using AlatrafClinic.Application.Features.Addresses.Queries.GetAddresses;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;


[Route("api/v{version:apiVersion}/addresses")]
[ApiVersion("1.0")]
public class AddressesController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<AddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of addresses.")]
    [EndpointDescription("Supports searching by address.")]
    [EndpointName("GetAddresses")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] string? searchTerm,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetAddressesQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            searchTerm
        );
         

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a specific address by its ID.")]
    [EndpointDescription("Retrieves a specific address by its ID.")]
    [EndpointName("GetAddressById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        int id,
        CancellationToken ct = default)
    {
        var query = new GetAddressQuery(
            id
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(AddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new address.")]
    [EndpointDescription("Creates a new address with the provided details.")]
    [EndpointName("CreateAddress")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create(
        [FromBody] CreateAddressRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateAddressCommand(
            request.Name
        );

        var result = await sender.Send(command, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing address.")]
    [EndpointDescription("Updates an existing address with the provided details.")]
    [EndpointName("UpdateAddress")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateAddressRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateAddressCommand(
            id,
            request.Name
        );

        var result = await sender.Send(command, ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
    
}