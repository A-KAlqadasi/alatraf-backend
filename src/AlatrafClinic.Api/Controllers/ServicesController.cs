using AlatrafClinic.Api.Requests.Services;
using AlatrafClinic.Application.Features.Services.Commands.CreateService;
using AlatrafClinic.Application.Features.Services.Commands.UpdateService;
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

    [HttpPost]
    [ProducesResponseType(typeof(ServiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new service.")]
    [EndpointDescription("Creates a new service under a specific department with an optional price and returns the created service details.")]
    [EndpointName("CreateService")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create(
        [FromBody] CreateServiceRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateServiceCommand(
            request.Name,
            request.DepartmentId,
            request.Price
        ), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetServiceById", // ensure this route exists
                routeValues: new { version = "1.0", serviceId = response.ServiceId },
                value: response),
            Problem
        );
    }

    [HttpPut("{serviceId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing service.")]
    [EndpointDescription("Updates the name, department, and optional price of an existing service using its unique identifier.")]
    [EndpointName("UpdateService")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(
        int serviceId,
        [FromBody] UpdateServiceRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateServiceCommand(
            serviceId,
            request.Name,
            request.DepartmentId,
            request.Price
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}