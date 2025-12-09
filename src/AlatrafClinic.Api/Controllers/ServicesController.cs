using AlatrafClinic.Application.Features.Services.Dtos;
using AlatrafClinic.Application.Features.Services.Queries.GetServiceById;
using AlatrafClinic.Application.Features.Services.Queries.GetServices;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/services")]
[ApiVersion("1.0")]
public sealed class ServicesController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ServiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve services.")]
    [EndpointDescription("Retrive all the services in the system.")]
    [EndpointName("GetServices")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetServicesQuery(), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{serviceId:int}", Name = "GetServiceById")]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve service by Id.")]
    [EndpointDescription("Retrive service by its Id")]
    [EndpointName("GetServiceById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int serviceId, CancellationToken ct)
    {
        var result = await sender.Send(new GetServiceByIdQuery(serviceId), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }


}