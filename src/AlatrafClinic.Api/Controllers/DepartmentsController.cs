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
    public async Task<IActionResult> GetById(int departmentId, CancellationToken ct)
    {
        var result = await sender.Send(new GetDepartmentSectionsQuery(departmentId), ct);
        return result.Match(
          response => Ok(response),
          Problem);
    }
}