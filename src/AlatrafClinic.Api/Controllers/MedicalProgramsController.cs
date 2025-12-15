using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.MedicalPrograms;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.MedicalPrograms.Commands.CreateMedicalProgram;
using AlatrafClinic.Application.Features.MedicalPrograms.Commands.DeleteMedicalProgram;
using AlatrafClinic.Application.Features.MedicalPrograms.Commands.UpdateMedicalProgram;
using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramById;
using AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalPrograms;
using AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramsWithFilters;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/medical-programs")]
[ApiVersion("1.0")]
public sealed class MedicalProgramsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<MedicalProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of medical programs.")]
    [EndpointDescription("Supports filtering medical programs by search term. Sorting is by name.")]
    [EndpointName("GetMedicalPrograms")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await sender.Send(new GetMedicalProgramsQuery(), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("with-filters")]
    [ProducesResponseType(typeof(PaginatedList<MedicalProgramDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of medical programs.")]
    [EndpointDescription(
        "Supports filtering medical programs by search term, section, and section assignment status. " +
        "Results are paginated and sortable."
    )]
    [EndpointName("GetMedicalProgramsWithFilters")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] MedicalProgramsFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetMedicalProgramsWithFilterQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.SectionId,
            filter.HasSection,
            filter.SortColumn,
            filter.SortDirection
        );

        var result = await sender.Send(query, ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }


    [HttpGet("{medicalProgramId:int}", Name = "GetMedicalProgramById")]
    [ProducesResponseType(typeof(MedicalProgramDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieve medical program by Id.")]
    [EndpointDescription("Retrive medical program by its Id")]
    [EndpointName("GetMedicalProgramById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int medicalProgramId, CancellationToken ct)
    {
        var result = await sender.Send(new GetMedicalProgramByIdQuery(medicalProgramId), ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(MedicalProgramDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new medical program.")]
    [EndpointDescription("Creates a new medical program with the specified details.")]
    [EndpointName("CreateMedicalProgram")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateMedicalProgramRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new CreateMedicalProgramCommand(request.Name, request.Description, request.SectionId), ct);
        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetMedicalProgramById",
                routeValues: new { version = "1.0", medicalProgramId = response.Id },
                value: response),
                Problem
        );
    }

    [HttpPut("{medicalProgramId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing medical program.")]
    [EndpointDescription("Updates an existing medical program with the specified details.")]
    [EndpointName("UpdateMedicalProgram")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int medicalProgramId, [FromBody] UpdateMedicalProgramRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateMedicalProgramCommand(medicalProgramId, request.Name, request.Description, request.SectionId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{medicalProgramId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing medical program.")]
    [EndpointDescription("Deletes an existing medical program by its Id.")]
    [EndpointName("DeleteMedicalProgram")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int medicalProgramId, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteMedicalProgramCommand(medicalProgramId), ct);
        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}