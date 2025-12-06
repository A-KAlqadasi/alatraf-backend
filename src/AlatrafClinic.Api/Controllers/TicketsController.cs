using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Tickets;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Queries.GetTickets;
using AlatrafClinic.Domain.Services.Enums;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/tickets")]
[ApiVersion("1.0")]
public sealed class TicketsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TicketDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of tickets.")]
    [EndpointDescription("Supports filtering tickets by search term, patient, service, department, status, and creation date range. Sorting is customizable.")]
    [EndpointName("GetTickets")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Get([FromQuery] TicketFilterRequest filters, [FromQuery] PageRequest pageRequest, CancellationToken ct)
    {
        if (pageRequest.Page <= 0)
        {
            return BadRequest("Page must be greater than 0");
        }

        if (pageRequest.PageSize <= 0 || pageRequest.PageSize > 100)
        {
            return BadRequest("PageSize must be between 1 and 100");
        }

        var query = new GetTicketsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filters.SearchTerm,
            filters.Status is not null ? (TicketStatus)(int)filters.Status : null,
            filters.PatientId,
            filters.ServiceId,
            filters.DepartmentId,
            filters.CreatedFrom,
            filters.CreatedTo,
            filters.SortBy,
            filters.SortDirection
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem);
    }

}