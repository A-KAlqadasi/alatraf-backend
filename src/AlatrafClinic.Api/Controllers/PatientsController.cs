using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.Patients;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.Patients.Commands.CreatePatient;
using AlatrafClinic.Application.Features.Patients.Commands.DeletePatient;
using AlatrafClinic.Application.Features.Patients.Commands.UpdatePatient;
using AlatrafClinic.Application.Features.Patients.Dtos;
using AlatrafClinic.Application.Features.Patients.Queries.GetPatientById;
using AlatrafClinic.Application.Features.Patients.Queries.GetPatients;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/patients")]
[ApiVersion("1.0")]
public sealed class PatientsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<PatientDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of patients.")]
    [EndpointDescription("Supports filtering patients by various criteria including search term, Gender, patientType, birthdate and HasNationalNo, sort is customizeable.")]
    [EndpointName("GetPatients")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] PatientFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetPatientsQuery(
            pageRequest.Page,
            pageRequest.PageSize,
            filter.SearchTerm,
            filter.PatientType,
            filter.Gender,
            filter.BirthDateFrom,
            filter.BirthDateTo,
            filter.HasNationalNo,
            filter.SortColumn,
            filter.SortDirection
        );

        var result = await sender.Send(query, ct);
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{patientId:int}", Name = "GetPatientById")]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a patient by its ID.")]
    [EndpointDescription("Fetches detailed information about a specific patient using its unique identifier.")]
    [EndpointName("GetPatientById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int patientId, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetPatientByIdQuery(patientId), ct);
        
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new patient.")]
    [EndpointDescription("Creates a new patient with the provided details and returns the created patient information.")]
    [EndpointName("CreatePatient")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreatePatientRequest request, CancellationToken ct = default)
    {

        var result = await sender.Send(new CreatePatientCommand(
            request.Fullname,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.Address,
            request.Gender,
            request.PatientType
        ), ct);

         return result.Match(
            response => CreatedAtRoute(
                routeName: "GetPatientById",
                routeValues: new { version = "1.0", patientId = response.PatientId },
                value: response),
                Problem
        );
    }

    [HttpPut("patientId:int")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Update patient.")]
    [EndpointDescription("Update an existing patient with the provided details.")]
    [EndpointName("UpdatePatient")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(int patientId, [FromBody] UpdatePatientRequest request, CancellationToken ct = default)
    {

        var result = await sender.Send(new UpdatePatientCommand(
            patientId,
            request.Fullname,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.Address,
            request.Gender,
            request.PatientType
        ), ct);

         return result.Match(
            _ => NoContent(),
                Problem
        );
    }

    [HttpDelete("{patientId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Delete a patient.")]
    [EndpointDescription("Deletes the specified patient from the system.")]
    [EndpointName("DeletePatient")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Delete(int patientId, CancellationToken ct)
    {
        var result = await sender.Send(new DeletePatientCommand(patientId), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }
    


}