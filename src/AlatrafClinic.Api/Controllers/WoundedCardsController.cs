
using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.WoundedCards;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.WoundedCards.Commands.AddWoundedCard;
using AlatrafClinic.Application.Features.WoundedCards.Commands.DeleteWoundedCard;
using AlatrafClinic.Application.Features.WoundedCards.Commands.UpdateWoundedCard;
using AlatrafClinic.Application.Features.WoundedCards.Dtos;
using AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCardByNumber;
using AlatrafClinic.Application.Features.WoundedCards.Queries.GetWoundedCards;

using Asp.Versioning;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/wounded-cards")]
[ApiVersion("1.0")]
public sealed class WoundedCardsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<WoundedCardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of wounded cards.")]
    [EndpointDescription("Supports filtering by search term, patientId, expiration range, and expired status. Sorting is customizable.")]
    [EndpointName("GetWoundedCards")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] WoundedCardsFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetWoundedCardsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.IsExpired,
            filter.PatientId,
            filter.IssueDateFrom,
            filter.IssueDateTo,
            filter.ExpirationFrom,
            filter.ExpirationTo,
            filter.SortColumn,
            filter.SortDirection
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("by-number/{cardNumber}", Name = "GetWoundedCardByNumber")]
    [ProducesResponseType(typeof(WoundedCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a wounded card by its card number.")]
    [EndpointDescription("Fetches detailed information for a wounded card using its unique card number.")]
    [EndpointName("GetWoundedCardByNumber")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetByNumber(
        [FromRoute][Required][StringLength(100, MinimumLength = 3)] string cardNumber,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetWoundedCardByNumberQuery(cardNumber), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(WoundedCardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new wounded card.")]
    [EndpointDescription("Creates a new wounded card for a patient and returns the created card details.")]
    [EndpointName("AddWoundedCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] AddWoundedCardRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new AddWoundedCardCommand(
            request.PatientId,
            request.CardNumber,
            request.IssueDate,
            request.ExpirationDate,
            request.CardImagePath
        ), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetWoundedCardByNumber",
                routeValues: new { version = "1.0", cardNumber = response.CardNumber },
                value: response),
            Problem
        );
    }

    [HttpPut("{woundedCardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing wounded card.")]
    [EndpointDescription("Updates an existing wounded card using its ID and the newly provided details.")]
    [EndpointName("UpdateWoundedCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(
        int woundedCardId,
        [FromBody] UpdateWoundedCardRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateWoundedCardCommand(
            woundedCardId,
            request.PatientId,
            request.CardNumber,
            request.IssueDate,
            request.ExpirationDate,
            request.CardImagePath
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{woundedCardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing wounded card.")]
    [EndpointDescription("Deletes a wounded card using its unique identifier.")]
    [EndpointName("DeleteWoundedCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Delete(int woundedCardId, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteWoundedCardCommand(woundedCardId), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}
