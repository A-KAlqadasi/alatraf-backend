using AlatrafClinic.Api.Requests.People;
using AlatrafClinic.Application.Features.People.Commands.CreatePerson;
using AlatrafClinic.Application.Features.People.Dtos;
using AlatrafClinic.Application.Features.People.Queries.GetPersonById;
using AlatrafClinic.Application.Features.People.Queries.GetPersonByNameOrNationalNoOrPhone;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/people")]
[ApiVersion("1.0")]
public class PeopleController (ISender sender) : ApiController
{
    [HttpGet("{personId:int}", Name = "GetPersonById")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a person by its ID.")]
    [EndpointDescription("Fetches detailed information about a specific person using its unique identifier.")]
    [EndpointName("GetPersonById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int personId, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetPersonByIdQuery(personId), ct);
        
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new person.")]
    [EndpointDescription("Creates a new person with the provided details and returns the created person information.")]
    [EndpointName("CreatePerson")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreatePersonRequest request, CancellationToken ct = default)
    {

        var result = await sender.Send(new CreatePersonCommand(
            request.Fullname,
            request.Birthdate,
            request.Phone,
            request.NationalNo,
            request.Address,
            request.Gender
        ), ct);

         return result.Match(
            response => CreatedAtRoute(
                routeName: "GetPersonById",
                routeValues: new { version = "1.0", personId = response.PersonId },
                value: response),
                Problem
        );
    }

    [HttpGet("search", Name = "GetPersonByNameOrNationalNoOrPhone")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a person by name, national number, or phone.")]
    [EndpointDescription("Fetches detailed information about a specific person using their name, national number, or phone number.")]
    [EndpointName("GetPersonByNameOrNationalNoOrPhone")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetByNameOrNationalNoOrPhone(
        [FromQuery] string? name,
        [FromQuery] string? nationalNo,
        [FromQuery] string? phone,
        CancellationToken cancellationToken)
    {
        var query = new GetPersonByNameOrNationalNoOrPhoneQuery(
            Name: name,
            NationalNo: nationalNo,
            Phone: phone
        );

        var result = await sender.Send(query, cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result);

        return Ok(result);
    }

}