

using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.DisabledCards;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.DisabledCards.Commands.AddDisabledCard;
using AlatrafClinic.Application.Features.DisabledCards.Commands.DeleteDisabledCard;
using AlatrafClinic.Application.Features.DisabledCards.Commands.UpdateDisabledCard;
using AlatrafClinic.Application.Features.DisabledCards.Dtos;
using AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCardByNumber;
using AlatrafClinic.Application.Features.DisabledCards.Queries.GetDisabledCards;

using Asp.Versioning;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/disabled-cards")]
[ApiVersion("1.0")]
public sealed class DisabledCardsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<DisabledCardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of disabled cards.")]
    [EndpointDescription("Supports filtering by search term, patientId, expiration range, and expired status. Sorting is customizable.")]
    [EndpointName("GetDisabledCards")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] DisabledCardsFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetDisabledCardsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.IsExpired,
            filter.PatientId,
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

    [HttpGet("by-number/{cardNumber}", Name = "GetDisabledCardByNumber")]
    [ProducesResponseType(typeof(DisabledCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a disabled card by its card number.")]
    [EndpointDescription("Fetches detailed information for a disabled card using its unique card number.")]
    [EndpointName("GetDisabledCardByNumber")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetByNumber(
        [FromRoute][Required][StringLength(100, MinimumLength = 3)] string cardNumber,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new GetDisabledCardByNumberQuery(cardNumber), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(DisabledCardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new disabled card.")]
    [EndpointDescription("Creates a new disabled card for a patient and returns the created card details.")]
    [EndpointName("AddDisabledCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] AddDisabledCardRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new AddDisabledCardCommand(
            request.PatientId,
            request.CardNumber,
            request.ExpirationDate,
            request.CardImagePath
        ), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetDisabledCardByNumber",
                routeValues: new { version = "1.0", cardNumber = response.CardNumber },
                value: response),
            Problem
        );
    }

    [HttpPut("{disabledCardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing disabled card.")]
    [EndpointDescription("Updates an existing disabled card using its ID and the newly provided details.")]
    [EndpointName("UpdateDisabledCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(
        int disabledCardId,
        [FromBody] UpdateDisabledCardRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateDisabledCardCommand(
            disabledCardId,
            request.PatientId,
            request.CardNumber,
            request.ExpirationDate,
            request.CardImagePath
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
    [HttpDelete("{disabledCardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing disabled card.")]
    [EndpointDescription("Deletes a disabled card using its unique identifier.")]
    [EndpointName("DeleteDisabledCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Delete(
        int disabledCardId,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new DeleteDisabledCardCommand(disabledCardId),
            ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

}
