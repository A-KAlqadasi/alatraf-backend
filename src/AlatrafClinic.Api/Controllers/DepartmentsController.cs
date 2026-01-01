using AlatrafClinic.Api.Requests.Departments;
using AlatrafClinic.Application.Features.Departments.Commands.DeleteDepartment;
using AlatrafClinic.Application.Features.Departments.Commands.UpdateDepartment;
using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Application.Features.Departments.Queries.GetDepartmentById;
using AlatrafClinic.Application.Features.Departments.Queries.GetDepartments;
using AlatrafClinic.Application.Features.Organization.Departments.Commands.CreateDepartment;
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Application.Features.Sections.Queries.GetDepartmentSections;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/departments")]
[ApiVersion("1.0")]
public sealed class DepartmentsController(ISender sender) : ApiController
{
    [HttpGet("{departmentId:int}/sections", Name = "GetDepartmentSections")]
    [ProducesResponseType(typeof(List<DepartmentSectionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves departments sections.")]
    [EndpointDescription("Returns a list of sections associated with the specified department.")]
    [EndpointName("GetDepartmentSections")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GetDepartmentSectionsById(int departmentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDepartmentSectionsQuery(departmentId), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<DepartmentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves the list of departments.")]
    [EndpointDescription("Returns all departments. Result is cached for improved performance.")]
    [EndpointName("GetDepartments")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetDepartmentsQuery(), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{departmentId:int}", Name = "GetDepartmentById")]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a department by its ID.")]
    [EndpointDescription("Fetches department details using its unique identifier.")]
    [EndpointName("GetDepartmentById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int departmentId, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetDepartmentByIdQuery(departmentId), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new department.")]
    [EndpointDescription("Creates a new department with the provided name and returns the created department details.")]
    [EndpointName("CreateDepartment")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateDepartmentCommand(request.Name), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetDepartmentById",
                routeValues: new { version = "1.0", departmentId = response.Id },
                value: response),
            Problem
        );
    }

    [HttpPut("{departmentId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing department.")]
    [EndpointDescription("Updates a department name using its unique identifier.")]
    [EndpointName("UpdateDepartment")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(
        int departmentId,
        [FromBody] UpdateDepartmentRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateDepartmentCommand(departmentId, request.NewName), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

    [HttpDelete("{departmentId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Deletes an existing department.")]
    [EndpointDescription("Deletes a department using its unique identifier.")]
    [EndpointName("DeleteDepartment")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Delete(int departmentId, CancellationToken ct = default)
    {
        var result = await sender.Send(new DeleteDepartmentCommand(departmentId), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }

}