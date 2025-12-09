using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Tickets;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Tickets.Commands.CreateTicket;
using AlatrafClinic.Application.Features.Tickets.Commands.DeleteTicket;
using AlatrafClinic.Application.Features.Tickets.Commands.UpdateTicket;
using AlatrafClinic.Application.Features.Tickets.Dtos;
using AlatrafClinic.Application.Features.Tickets.Queries.GetTicketById;
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

    [HttpGet("{ticketId:int}", Name = "GetTicketById")]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a ticket by its ID.")]
    [EndpointDescription("Returns detailed information about the specified ticket if it exists.")]
    [EndpointName("GetTicketById")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetById(int ticketId, CancellationToken ct)
    {
        var result = await sender.Send(new GetTicketByIdQuery(ticketId), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TicketDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new ticket.")]
    [EndpointDescription("Creates a new ticket, specifying service, and patient if service is not consultation")]
    [EndpointName("CreateTicket")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateTicketCommand(request.ServiceId, request.PatientId), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetTicketById",
                routeValues: new { version = "1.0", ticketId = response.TicketId },
                value: response),
                Problem
        );
    }

    [HttpPut("{ticketId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing ticket.")]
    [EndpointDescription("Updates the service, patient, and optionally status of an existing ticket.")]
    [EndpointName("UpdateTicket")]
    [MapToApiVersion("1.0")]
    
    public async Task<IActionResult> Update(int ticketId, [FromBody] UpdateTicketRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateTicketCommand(ticketId, request.ServiceId, request.PatientId, request.Status), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpDelete("{ticketId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes a ticket by its ID.")]
    [EndpointDescription("Removes the specified ticket from the system if it exists.")]
    [EndpointName("DeleteTicket")]
    public async Task<IActionResult> Delete(int ticketId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteTicketCommand(ticketId), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

}