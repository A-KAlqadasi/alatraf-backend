using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Addresses.Dtos;
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
}