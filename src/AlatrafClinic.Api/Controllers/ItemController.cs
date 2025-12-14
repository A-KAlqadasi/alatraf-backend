using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;

using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.Items.Dtos;

using AlatrafClinic.Application.Features.Inventory.Items.Commands.CreateItemCommand;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.UpdateItemCommand;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.DeleteItemCommand;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.ActivateItemCommand;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.DeactivateItemCommand;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.AddOrUpdateItemUnitCommand;
using AlatrafClinic.Application.Features.Inventory.Items.Commands.RemoveItemUnitCommand;

using AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsListQuery;
using AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemsWithUnitsQuery;
using AlatrafClinic.Application.Features.Inventory.Items.Queries.GetInactiveItemsQuery;
using AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemByIdQuery;
using AlatrafClinic.Application.Features.Inventory.Items.Queries.GetItemUnitsByItemIdQuery;
using AlatrafClinic.Application.Features.Inventory.Items.Queries.SearchItemsQuery;

using Microsoft.AspNetCore.Http;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/items")]
[ApiVersion("1.0")]
public sealed class ItemController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve items.")]
    [EndpointDescription("Retrieves all items from the system.")]
    [EndpointName("GetItemsList")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetItemsListQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("with-units")]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve items with units.")]
    [EndpointDescription("Retrieves all items including their units information.")]
    [EndpointName("GetItemsWithUnits")]
    public async Task<IActionResult> GetWithUnits(CancellationToken ct)
    {
        var result = await sender.Send(new GetItemsWithUnitsQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("inactive")]
    [ProducesResponseType(typeof(List<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve inactive items.")]
    [EndpointDescription("Retrieves items that are marked as inactive.")]
    [EndpointName("GetInactiveItems")]
    public async Task<IActionResult> GetInactive(CancellationToken ct)
    {
        var result = await sender.Send(new GetInactiveItemsQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{id:int}", Name = "GetItemById")]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve item by Id.")]
    [EndpointDescription("Retrieves an item by its identifier.")]
    [EndpointName("GetItemById")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await sender.Send(new GetItemByIdQuery(id), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{id:int}/units")]
    [ProducesResponseType(typeof(List<ItemUnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve item units by item id.")]
    [EndpointDescription("Retrieves units for a given item id.")]
    [EndpointName("GetItemUnitsByItemId")]
    public async Task<IActionResult> GetItemUnitsByItemId(int id, CancellationToken ct)
    {
        var result = await sender.Send(new GetItemUnitsByItemIdQuery(id), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(PagedList<ItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Search items.")]
    [EndpointDescription("Searches items with filters, pagination and sorting.")]
    [EndpointName("SearchItems")]
    public async Task<IActionResult> Search([FromQuery] SearchItemsQuery query, CancellationToken ct)
    {
        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(ItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new item.")]
    [EndpointDescription("Creates a new inventory item with the provided details.")]
    [EndpointName("CreateItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateItemCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetItemById",
                routeValues: new { version = "1.0", id = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing item.")]
    [EndpointDescription("Updates an existing inventory item with the specified details.")]
    [EndpointName("UpdateItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateItemCommand command, CancellationToken ct)
    {
        var cmd = new UpdateItemCommand(id, command.Name, command.Description);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing item.")]
    [EndpointDescription("Deletes an existing inventory item by id.")]
    [EndpointName("DeleteItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteItemCommand(id), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{id:int}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Activates an item.")]
    [EndpointDescription("Marks an item as active.")]
    [EndpointName("ActivateItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Activate(int id, CancellationToken ct)
    {
        var result = await sender.Send(new ActivateItemCommand(id), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{id:int}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deactivates an item.")]
    [EndpointDescription("Marks an item as inactive.")]
    [EndpointName("DeactivateItem")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Deactivate(int id, CancellationToken ct)
    {
        var result = await sender.Send(new DeactivateItemCommand(id), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPut("{id:int}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Add or update an item unit.")]
    [EndpointDescription("Adds a new unit to an item or updates an existing one.")]
    [EndpointName("AddOrUpdateItemUnit")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AddOrUpdateItemUnit(int id, [FromBody] AddOrUpdateItemUnitCommand command, CancellationToken ct)
    {
        var cmd = new AddOrUpdateItemUnitCommand(id, command.UnitId, command.Price, command.ConversionFactor, command.MinPriceToPay, command.MaxPriceToPay);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{id:int}/items/{unitId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Remove an item unit.")]
    [EndpointDescription("Removes a specific unit from an item.")]
    [EndpointName("RemoveItemUnit")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RemoveItemUnit(int id, int unitId, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveItemUnitCommand(id, unitId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
