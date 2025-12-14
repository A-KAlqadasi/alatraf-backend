using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Sales.Dtos;

using AlatrafClinic.Application.Features.Sales.Commands.CreateSale;
using AlatrafClinic.Application.Features.Sales.Commands.UpdateSale;
using AlatrafClinic.Application.Features.Sales.Commands.DeleteSale;

using AlatrafClinic.Application.Features.Sales.Queries.GetSales;
using AlatrafClinic.Application.Features.Sales.Queries.GetSaleById;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/sales")]
[ApiVersion("1.0")]
public sealed class SalesController(ISender sender) : ApiController
{
    // Queries
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<SaleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve sales.")]
    [EndpointDescription("Retrieves a paginated list of sales with optional filters.")]
    [EndpointName("GetSales")]
    public async Task<IActionResult> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? searchTerm = null, [FromQuery] int? diagnosisId = null, [FromQuery] int? patientId = null, [FromQuery] DateOnly? fromDate = null, [FromQuery] DateOnly? toDate = null, [FromQuery] string sortColumn = "SaleDate", [FromQuery] string sortDirection = "desc", CancellationToken ct = default)
    {
        var query = new GetSalesQuery(page, pageSize, searchTerm, null, diagnosisId, patientId, fromDate, toDate, sortColumn, sortDirection);
        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{saleId:int}", Name = "GetSaleById")]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve sale by Id.")]
    [EndpointDescription("Retrieves a sale by its identifier.")]
    [EndpointName("GetSaleById")]
    public async Task<IActionResult> GetById(int saleId, CancellationToken ct)
    {
        var result = await sender.Send(new GetSaleByIdQuery(saleId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    // Commands
    [HttpPost]
    [ProducesResponseType(typeof(SaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new sale.")]
    [EndpointDescription("Creates a new sale with header and items.")]
    [EndpointName("CreateSale")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetSaleById",
                routeValues: new { version = "1.0", saleId = response.SaleId },
                value: response),
            Problem
        );
    }

    [HttpPut("{saleId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing sale.")]
    [EndpointDescription("Updates sale header and items.")]
    [EndpointName("UpdateSale")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int saleId, [FromBody] UpdateSaleCommand command, CancellationToken ct)
    {
        var cmd = new UpdateSaleCommand(saleId, command.TicketId, command.DiagnosisText, command.InjuryDate, command.InjuryReasons, command.InjurySides, command.InjuryTypes, command.SaleItems, command.Notes);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{saleId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing sale.")]
    [EndpointDescription("Deletes the specified sale.")]
    [EndpointName("DeleteSale")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int saleId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteSaleCommand(saleId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
