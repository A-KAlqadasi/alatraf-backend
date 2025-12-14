using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AlatrafClinic.Application.Features.Inventory.Purchases.Dtos;

using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoice;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CreatePurchaseInvoiceWithItems;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseInvoice;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.SubmitPurchaseInvoice;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.ApprovePurchaseInvoice;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.CancelPurchaseInvoice;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.AddPurchaseItem;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.UpdatePurchaseItem;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.RemovePurchaseItem;
using AlatrafClinic.Application.Features.Inventory.Purchases.Commands.MarkPurchaseInvoicePaid;

using AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetAllPurchaseInvoices;
using AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceById;
using AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoiceItems;
using AlatrafClinic.Application.Features.Inventory.Purchases.Queries.GetPurchaseInvoicesSummary;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/purchases")]
[ApiVersion("1.0")]
public sealed class PurchasesController(ISender sender) : ApiController
{

    // Queries
    [HttpGet]
    [ProducesResponseType(typeof(List<PurchaseInvoiceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve purchase invoices.")]
    [EndpointDescription("Retrieves all purchase invoices from the system.")]
    [EndpointName("GetAllPurchaseInvoices")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetAllPurchaseInvoicesQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{purchaseInvoiceId:int}", Name = "GetPurchaseInvoiceById")]
    [ProducesResponseType(typeof(PurchaseInvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve purchase invoice by Id.")]
    [EndpointDescription("Retrieves a purchase invoice by its identifier.")]
    [EndpointName("GetPurchaseInvoiceById")]
    public async Task<IActionResult> GetById(int purchaseInvoiceId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPurchaseInvoiceByIdQuery(purchaseInvoiceId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{purchaseInvoiceId:int}/items")]
    [ProducesResponseType(typeof(List<PurchaseItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve items for a purchase invoice.")]
    [EndpointDescription("Retrieves all items for a given purchase invoice.")]
    [EndpointName("GetPurchaseInvoiceItems")]
    public async Task<IActionResult> GetItems(int purchaseInvoiceId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPurchaseInvoiceItemsQuery(purchaseInvoiceId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(List<PurchaseInvoiceSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve purchase invoices summary.")]
    [EndpointDescription("Retrieves a summary of purchase invoices with optional filters.")]
    [EndpointName("GetPurchaseInvoicesSummary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateTime? dateFrom, [FromQuery] DateTime? dateTo, [FromQuery] int? supplierId, [FromQuery] int? storeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetPurchaseInvoicesSummaryQuery(dateFrom, dateTo, supplierId, storeId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    // Commands
    [HttpPost]
    [ProducesResponseType(typeof(PurchaseInvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new purchase invoice.")]
    [EndpointDescription("Creates a new purchase invoice with the provided header details.")]
    [EndpointName("CreatePurchaseInvoice")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseInvoiceCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetPurchaseInvoiceById",
                routeValues: new { version = "1.0", purchaseInvoiceId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPost("with-items")]
    [ProducesResponseType(typeof(PurchaseInvoiceDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new purchase invoice with items.")]
    [EndpointDescription("Creates a new purchase invoice and adds line items in one request.")]
    [EndpointName("CreatePurchaseInvoiceWithItems")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> CreateWithItems([FromBody] CreatePurchaseInvoiceWithItemsCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetPurchaseInvoiceById",
                routeValues: new { version = "1.0", purchaseInvoiceId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPut("{purchaseInvoiceId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing purchase invoice.")]
    [EndpointDescription("Updates the header details of an existing purchase invoice.")]
    [EndpointName("UpdatePurchaseInvoice")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int purchaseInvoiceId, [FromBody] UpdatePurchaseInvoiceCommand command, CancellationToken ct)
    {
        var cmd = new UpdatePurchaseInvoiceCommand(purchaseInvoiceId, command.Number, command.Date, command.SupplierId, command.StoreId);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{purchaseInvoiceId:int}/submit")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Submit a purchase invoice.")]
    [EndpointDescription("Submits a purchase invoice for approval.")]
    [EndpointName("SubmitPurchaseInvoice")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Submit(int purchaseInvoiceId, CancellationToken ct)
    {
        var result = await sender.Send(new SubmitPurchaseInvoiceCommand(purchaseInvoiceId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{purchaseInvoiceId:int}/approve")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Approve a purchase invoice.")]
    [EndpointDescription("Approves a submitted purchase invoice.")]
    [EndpointName("ApprovePurchaseInvoice")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Approve(int purchaseInvoiceId, CancellationToken ct)
    {
        var result = await sender.Send(new ApprovePurchaseInvoiceCommand(purchaseInvoiceId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{purchaseInvoiceId:int}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Cancel a purchase invoice.")]
    [EndpointDescription("Cancels a submitted or approved purchase invoice.")]
    [EndpointName("CancelPurchaseInvoice")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Cancel(int purchaseInvoiceId, CancellationToken ct)
    {
        var result = await sender.Send(new CancelPurchaseInvoiceCommand(purchaseInvoiceId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{purchaseInvoiceId:int}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Add an item to a purchase invoice.")]
    [EndpointDescription("Adds a line item to the specified purchase invoice.")]
    [EndpointName("AddPurchaseItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AddItem(int purchaseInvoiceId, [FromBody] AddPurchaseItemCommand command, CancellationToken ct)
    {
        var cmd = new AddPurchaseItemCommand(purchaseInvoiceId, command.StoreItemUnitId, command.Quantity, command.UnitPrice, command.Notes);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPut("{purchaseInvoiceId:int}/items/{existingStoreItemUnitId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update an item on a purchase invoice.")]
    [EndpointDescription("Updates a line item on the specified purchase invoice.")]
    [EndpointName("UpdatePurchaseItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> UpdateItem(int purchaseInvoiceId, int existingStoreItemUnitId, [FromBody] UpdatePurchaseItemCommand command, CancellationToken ct)
    {
        var cmd = new UpdatePurchaseItemCommand(purchaseInvoiceId, existingStoreItemUnitId, command.NewStoreItemUnitId, command.Quantity, command.UnitPrice, command.Notes);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{purchaseInvoiceId:int}/items/{storeItemUnitId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Remove an item from a purchase invoice.")]
    [EndpointDescription("Removes a line item from the specified purchase invoice.")]
    [EndpointName("RemovePurchaseItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RemoveItem(int purchaseInvoiceId, int storeItemUnitId, CancellationToken ct)
    {
        var result = await sender.Send(new RemovePurchaseItemCommand(purchaseInvoiceId, storeItemUnitId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{purchaseInvoiceId:int}/pay")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Mark purchase invoice as paid.")]
    [EndpointDescription("Marks a purchase invoice as paid with payment details.")]
    [EndpointName("MarkPurchaseInvoicePaid")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> MarkPaid(int purchaseInvoiceId, [FromBody] MarkPurchaseInvoicePaidCommand command, CancellationToken ct)
    {
        var cmd = new MarkPurchaseInvoicePaidCommand(purchaseInvoiceId, command.Amount, command.Method, command.Reference);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
