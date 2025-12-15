using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.ExchangeOrders.Dtos;

using AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForOrder;
using AlatrafClinic.Application.Features.Inventory.Commands.CreateExchangeOrderForSale;
using AlatrafClinic.Application.Features.Inventory.Commands.UpsertExchangeOrderItems;
using AlatrafClinic.Application.Features.Inventory.Commands.ApproveExchangeOrder;
using AlatrafClinic.Application.Features.Inventory.Commands.CancelExchangeOrder;

using AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrders;
using AlatrafClinic.Application.Features.Inventory.Queries.GetAllExchangeOrders;
using AlatrafClinic.Application.Features.Inventory.Queries.GetExchangeOrderById;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/exchange-orders")]
[ApiVersion("1.0")]
public sealed class ExchangeOrdersController(ISender sender) : ApiController
{
    // Queries
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<ExchangeOrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve exchange orders (paged).")]
    [EndpointDescription("Retrieves a paginated list of exchange orders with optional filters.")]
    [EndpointName("GetExchangeOrders")]
    public async Task<IActionResult> Get(
        [FromQuery] int? orderId,
        [FromQuery] int? saleId,
        [FromQuery] int? storeId,
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortDirection,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var query = new GetExchangeOrdersQuery(orderId, saleId, storeId, searchTerm, sortColumn, sortDirection, page, pageSize);
        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(List<ExchangeOrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve all exchange orders." )]
    [EndpointDescription("Retrieves all exchange orders with optional filters.")]
    [EndpointName("GetAllExchangeOrders")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? storeId,
        [FromQuery] int? saleId,
        [FromQuery] int? orderId,
        [FromQuery] string? searchTerm,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortDirection,
        CancellationToken ct = default)
    {
        var query = new GetAllExchangeOrdersQuery(storeId, saleId, orderId, searchTerm, sortColumn, sortDirection);
        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{exchangeOrderId:int}", Name = "GetExchangeOrderById")]
    [ProducesResponseType(typeof(ExchangeOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve exchange order by Id.")]
    [EndpointDescription("Retrieves an exchange order by its identifier.")]
    [EndpointName("GetExchangeOrderById")]
    public async Task<IActionResult> GetById(int exchangeOrderId, CancellationToken ct)
    {
        var result = await sender.Send(new GetExchangeOrderByIdQuery(exchangeOrderId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    // Commands
    [HttpPost("for-order")]
    [ProducesResponseType(typeof(ExchangeOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates an exchange order for an order.")]
    [EndpointDescription("Creates an exchange order linked to a repair order with provided items.")]
    [EndpointName("CreateExchangeOrderForOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> CreateForOrder([FromBody] CreateExchangeOrderForOrderCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetExchangeOrderById",
                routeValues: new { version = "1.0", exchangeOrderId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPost("for-sale")]
    [ProducesResponseType(typeof(ExchangeOrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates an exchange order for a sale.")]
    [EndpointDescription("Creates an exchange order linked to a sale with provided items.")]
    [EndpointName("CreateExchangeOrderForSale")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> CreateForSale([FromBody] CreateExchangeOrderForSaleCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetExchangeOrderById",
                routeValues: new { version = "1.0", exchangeOrderId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPut("{exchangeOrderId:int}/items")]
    [ProducesResponseType(typeof(ExchangeOrderDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Upsert items for an exchange order.")]
    [EndpointDescription("Upserts line items for the specified exchange order.")]
    [EndpointName("UpsertExchangeOrderItems")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpsertItems(int exchangeOrderId, [FromBody] UpsertExchangeOrderItemsCommand command, CancellationToken ct)
    {
        var cmd = new UpsertExchangeOrderItemsCommand(exchangeOrderId, command.Items);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost("{exchangeOrderId:int}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Approve an exchange order.")]
    [EndpointDescription("Approves the specified exchange order.")]
    [EndpointName("ApproveExchangeOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Approve(int exchangeOrderId, CancellationToken ct)
    {
        var result = await sender.Send(new ApproveExchangeOrderCommand(exchangeOrderId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{exchangeOrderId:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Cancel an exchange order.")]
    [EndpointDescription("Cancels the specified exchange order.")]
    [EndpointName("CancelExchangeOrder")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Cancel(int exchangeOrderId, CancellationToken ct)
    {
        var result = await sender.Send(new CancelExchangeOrderCommand(exchangeOrderId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
