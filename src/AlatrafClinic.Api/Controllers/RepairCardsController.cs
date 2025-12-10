using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.RepairCards;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.RepairCards.Commands.CreateRepairCard;
using AlatrafClinic.Application.Features.RepairCards.Commands.UpdateRepairCard;
using AlatrafClinic.Application.Features.RepairCards.Dtos;
using AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCardById;
using AlatrafClinic.Application.Features.RepairCards.Queries.GetRepairCards;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/repair-cards")]
[ApiVersion("1.0")]
public sealed class RepairCardsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<RepairCardDiagnosisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of repair cards.")]
    [EndpointDescription("Supports filtering repair cards by various criteria including search term, status, DiagnosisId, IsActive, IsLate, and patient. Sorting is customizable.")]
    [EndpointName("GetRepairCards")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] RepairCardFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetRepairCardsQuery(
            Page: pageRequest.Page,
            PageSize: pageRequest.PageSize,
            SearchTerm: filter.SearchTerm,
            IsActive: filter.IsActive,
            IsLate: filter.IsLate,
            Status: filter.Status,
            DiagnosisId: filter.DiagnosisId,
            PatientId: filter.PatientId,
            SortColumn: filter.SortColumn,
            SortDirection: filter.SortDirection
        );

        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{repairCardId:int}", Name = "GetRepairCardById")]
    [ProducesResponseType(typeof(PaginatedList<RepairCardDiagnosisDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a repair card by its ID.")]
    [EndpointDescription("Fetches detailed information about a specific repair card using its unique identifier.")]
    [EndpointName("GetRepairCardById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        int repairCardId,
        CancellationToken ct = default)
    {
       
        var result = await sender.Send(new GetRepairCardByIdQuery(repairCardId), ct);
        
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(RepairCardDiagnosisDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new repair card.")]
    [EndpointDescription("Creates a new repair card with the provided details and returns the created repair card information.")]
    [EndpointName("CreateRepairCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateRepairCardRequest request, CancellationToken ct = default)
    {
        var industrialParts = request.IndustrialParts.ConvertAll(p => new CreateRepairCardIndustrialPartCommand(p.IndustrialPartId, p.UnitId, p.Quantity));
        
        var result = await sender.Send(new CreateRepairCardCommand(
            TicketId: request.TicketId,
            DiagnosisText: request.DiagnosisText,
            request.InjuryDate,
            request.InjuryReasons,
            request.InjurySides,
            request.InjuryTypes,
            industrialParts,
            request.Notes
        ), ct);

         return result.Match(
            response => CreatedAtRoute(
                routeName: "GetRepairCardById",
                routeValues: new { version = "1.0", repairCardId = response.RepairCardId },
                value: response),
                Problem
        );
    }

    [HttpPut("{repairCardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update a repair card.")]
    [EndpointDescription("Updates an existing repair card with the provided details.")]
    [EndpointName("UpdateRepairCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(int repairCardId, [FromBody] UpdateRepairCardRequest request, CancellationToken ct = default)
    {
        var industrialParts = request.IndustrialParts.ConvertAll(p => new UpdateRepairCardIndustrialPartCommand(p.IndustrialPartId, p.UnitId, p.Quantity));
        
        var result = await sender.Send(new UpdateRepairCardCommand(
            RepairCardId: repairCardId,
            TicketId: request.TicketId,
            DiagnosisText: request.DiagnosisText,
            request.InjuryDate,
            request.InjuryReasons,
            request.InjurySides,
            request.InjuryTypes,
            industrialParts,
            request.Notes
        ), ct);

         return result.Match(
            response => NoContent(),
                Problem
        );
    }

    


}