using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;

using AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.CreateSupplierCommand;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.UpdateSupplierCommand;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.DeleteSupplierCommand;

using AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetAllSuppliersQuery;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetSupplierByIdQuery;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/suppliers")]
[ApiVersion("1.0")]
public sealed class SuppliersController(ISender sender) : ApiController
{
    // Queries
    [HttpGet]
    [ProducesResponseType(typeof(List<SupplierDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve suppliers.")]
    [EndpointDescription("Retrieves all suppliers from the system." )]
    [EndpointName("GetAllSuppliers")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetAllSuppliersQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{supplierId:int}", Name = "GetSuppliersById")]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve supplier by Id.")]
    [EndpointDescription("Retrieves a supplier by its identifier.")]
    [EndpointName("GetSuppliersById")]
    public async Task<IActionResult> GetById(int supplierId, CancellationToken ct)
    {
        var result = await sender.Send(new GetSupplierByIdQuery(supplierId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    // Commands
    [HttpPost]
    [ProducesResponseType(typeof(SupplierDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new supplier.")]
    [EndpointDescription("Creates a new supplier with the provided details.")]
    [EndpointName("CreateSupplier")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetSuppliersById",
                routeValues: new { version = "1.0", supplierId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPut("{supplierId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing supplier.")]
    [EndpointDescription("Updates an existing supplier's details.")]
    [EndpointName("UpdateSuppliers")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int supplierId, [FromBody] UpdateSupplierCommand command, CancellationToken ct)
    {
        var cmd = new UpdateSupplierCommand(supplierId, command.SupplierName, command.Phone);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{supplierId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing supplier.")]
    [EndpointDescription("Deletes the specified supplier.")]
    [EndpointName("DeleteSuppliers")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int supplierId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteSupplierCommand(supplierId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
