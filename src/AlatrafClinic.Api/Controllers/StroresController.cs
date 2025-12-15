using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Inventory.Stores.Dtos;

using AlatrafClinic.Application.Features.Inventory.Stores.Commands.CreateStore;
using AlatrafClinic.Application.Features.Inventory.Stores.Commands.UpdateStore;
using AlatrafClinic.Application.Features.Inventory.Stores.Commands.DeleteStore;
using AlatrafClinic.Application.Features.Inventory.Stores.Commands.AddItemUnitToStore;
using AlatrafClinic.Application.Features.Inventory.Stores.Commands.RemoveItemUnitFromStore;
using AlatrafClinic.Application.Features.Inventory.Stores.Commands.AdjustStock;
using AlatrafClinic.Application.Features.Inventory.Stores.Commands.TransferStock;

using AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetAllStoresQuery;
using AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreByIdQuery;
using AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetStoreItemUnitsQuery;
using AlatrafClinic.Application.Features.Inventory.Stores.Queries.GetItemUnitQuantityInStoreQuery;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/strores")]
[ApiVersion("1.0")]
public sealed class StroresController(ISender sender) : ApiController
{
    // Queries
    [HttpGet]
    [ProducesResponseType(typeof(List<StoreDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve stores.")]
    [EndpointDescription("Retrieves all stores from the system.")]
    [EndpointName("GetAllStores")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetAllStoresQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }


    [HttpGet("{storeId:int}", Name = "GetStoreById")]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve store by Id.")]
    [EndpointDescription("Retrieves a store by its identifier.")]
    [EndpointName("GetStoreById")]
    public async Task<IActionResult> GetById(int storeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetStoreByIdQuery(storeId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{storeId:int}/items")]
    [ProducesResponseType(typeof(List<StoreItemUnitDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve store item units.")]
    [EndpointDescription("Retrieves all item units for a given store.")]
    [EndpointName("GetStoreItemUnits")]
    public async Task<IActionResult> GetStoreItemUnits(int storeId, CancellationToken ct)
    {
        var result = await sender.Send(new GetStoreItemUnitsQuery(storeId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{storeId:int}/items/{itemId:int}/units/{unitId:int}/quantity")]
    [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve item unit quantity in a store.")]
    [EndpointDescription("Retrieves the quantity for a specific item/unit within a store.")]
    [EndpointName("GetItemUnitQuantityInStore")]
    public async Task<IActionResult> GetItemUnitQuantity(int storeId, int itemId, int unitId, CancellationToken ct)
    {
        var result = await sender.Send(new GetItemUnitQuantityInStoreQuery(storeId, itemId, unitId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    // Commands
    [HttpPost]
    [ProducesResponseType(typeof(StoreDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new store.")]
    [EndpointDescription("Creates a new store with the provided details.")]
    [EndpointName("CreateStore")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateStoreCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetStoreById",
                routeValues: new { version = "1.0", storeId = response.StoreId },
                value: response),
            Problem
        );
    }

    [HttpPut("{storeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing store.")]
    [EndpointDescription("Updates an existing store's details.")]
    [EndpointName("UpdateStore")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int storeId, [FromBody] UpdateStoreCommand command, CancellationToken ct)
    {
        var cmd = new UpdateStoreCommand(storeId, command.Name);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{storeId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing store.")]
    [EndpointDescription("Deletes the specified store.")]
    [EndpointName("DeleteStore")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int storeId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteStoreCommand(storeId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("{storeId:int}/items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Add an item unit to a store.")]
    [EndpointDescription("Adds an item unit with a quantity to the specified store.")]
    [EndpointName("AddItemUnitToStore")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AddItemUnit(int storeId, [FromBody] AddItemUnitToStoreCommand command, CancellationToken ct)
    {
        var cmd = new AddItemUnitToStoreCommand(storeId, command.ItemId, command.UnitId, command.Quantity);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPut("{storeId:int}/items/{itemId:int}/units/{unitId:int}/adjust")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Adjust stock for an item unit in a store.")]
    [EndpointDescription("Adjusts stock (increase or decrease) for a specific item/unit in a store.")]
    [EndpointName("AdjustStock")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> AdjustStock(int storeId, int itemId, int unitId, [FromBody] AdjustStockCommand command, CancellationToken ct)
    {
        var cmd = new AdjustStockCommand(storeId, itemId, unitId, command.Quantity, command.Increase);
        var result = await sender.Send(cmd, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{storeId:int}/items/{itemId:int}/units/{unitId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Remove an item unit from a store.")]
    [EndpointDescription("Removes a specific item/unit from a store.")]
    [EndpointName("RemoveItemUnitFromStore")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> RemoveItemUnit(int storeId, int itemId, int unitId, CancellationToken ct)
    {
        var result = await sender.Send(new RemoveItemUnitFromStoreCommand(storeId, itemId, unitId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Transfer stock between stores.")]
    [EndpointDescription("Transfers stock of an item/unit from one store to another.")]
    [EndpointName("TransferStock")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Transfer([FromBody] TransferStockCommand command, CancellationToken ct)
    {
        var result = await sender.Send(command, ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
