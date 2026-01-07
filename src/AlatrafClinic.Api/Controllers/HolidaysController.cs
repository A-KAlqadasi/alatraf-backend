using AlatrafClinic.Api.Controllers;
using AlatrafClinic.Api.Requests.Holidays;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Holidays.Commands.CreateHoliday;
using AlatrafClinic.Application.Features.Holidays.Commands.DeleteHoliday;
using AlatrafClinic.Application.Features.Holidays.Commands.UpdateHoliday;
using AlatrafClinic.Application.Features.Holidays.Dtos;
using AlatrafClinic.Application.Features.Holidays.Queries.GetHoliday;
using AlatrafClinic.Application.Features.Holidays.Queries.GetHolidays;
using AlatrafClinic.Domain.Services.Enums;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

[Route("api/v{version:apiVersion}/holidays")]
[ApiVersion("1.0")]
public sealed class HolidaysController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<HolidayDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of holidays with filtering options.")]
    [EndpointDescription("Returns holidays with support for pagination, filtering by active status, date range, holiday type, and sorting.")]
    [EndpointName("GetHolidays")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isActive = null,
        [FromQuery] DateOnly? specificDate = null,
        [FromQuery] DateOnly? endDate = null,
        [FromQuery] HolidayType? type = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "desc",
        CancellationToken ct = default)
    {
        var query = new GetHolidaysQuery(
            page,
            pageSize,
            isActive,
            specificDate,
            endDate,
            type,
            sortBy,
            sortDirection);

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{id:int}", Name = "GetHolidayById")]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a holiday by its ID.")]
    [EndpointDescription("Fetches holiday details using its unique identifier.")]
    [EndpointName("GetHolidayById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetHolidayQuery(id), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(HolidayDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new holiday.")]
    [EndpointDescription("Creates a holiday with the provided details.")]
    [EndpointName("CreateHoliday")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create(
        [FromBody] CreateHolidayRequest request,
        CancellationToken ct = default)
    {
        var command = new CreateHolidayCommand(
            request.StartDate,
            request.EndDate,
            request.Name,
            request.IsRecurring,
            request.Type,
            request.IsActive);

        var result = await sender.Send(command, ct);

        return result.Match(
            response => CreatedAtAction(
                nameof(GetById),
                new { id = response.HolidayId },
                response),
            Problem
        );
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing holiday.")]
    [EndpointDescription("Updates a holiday with the provided details using its unique identifier.")]
    [EndpointName("UpdateHoliday")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpdateHolidayRequest request,
        CancellationToken ct = default)
    {
        var command = new UpdateHolidayCommand(
            id,
            request.Name,
            request.StartDate,
            request.EndDate,
            request.IsRecurring,
            request.Type,
            request.IsActive);

        var result = await sender.Send(command, ct);

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
    [EndpointSummary("Deletes an existing holiday.")]
    [EndpointDescription("Deletes a holiday using its unique identifier.")]
    [EndpointName("DeleteHoliday")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteHolidayCommand(id), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}