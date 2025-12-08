using AlatrafClinic.Api.Requests.Common;
using AlatrafClinic.Api.Requests.TherapyCards;
using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapyCard;
using AlatrafClinic.Application.Features.TherapyCards.Commands.CreateTherapySession;
using AlatrafClinic.Application.Features.TherapyCards.Commands.GenerateSessions;
using AlatrafClinic.Application.Features.TherapyCards.Commands.RenewTherapyCard;
using AlatrafClinic.Application.Features.TherapyCards.Commands.UpdateTherapyCard;
using AlatrafClinic.Application.Features.TherapyCards.Dtos;
using AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCardById;
using AlatrafClinic.Application.Features.TherapyCards.Queries.GetTherapyCards;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/therapy-cards")]
[ApiVersion("1.0")]
public sealed class TherapyCardsController(ISender sender) : ApiController
{
    
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedList<TherapyCardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a paginated list of therapy cards.")]
    [EndpointDescription("Supports filtering therapy cards by various criteria including search term, status, type, program start and end dates, diagnosis, and patient. Sorting is customizable.")]
    [EndpointName("GetTherapyCards")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Get(
        [FromQuery] TherapyCardFilterRequest filter,
        [FromQuery] PageRequest pageRequest,
        CancellationToken ct = default)
    {
        var query = new GetTherapyCardsQuery(
            Page: pageRequest.Page,
            PageSize: pageRequest.PageSize,
            SearchTerm: filter.SearchTerm,
            IsActive: filter.IsActive,
            Type: filter.TherapyCardType,
            Status: filter.TherapyCardStatus,
            ProgramStartFrom: filter.ProgramStartFrom,
            ProgramStartTo: filter.ProgramStartTo,
            ProgramEndFrom: filter.ProgramEndFrom,
            ProgramEndTo: filter.ProgramEndTo,
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

    [HttpGet("{therapyCardId:int}", Name = "GetTherapyCardById")]
    [ProducesResponseType(typeof(TherapyCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves a therapy card by its ID.")]
    [EndpointDescription("Fetches detailed information about a specific therapy card using its unique identifier.")]
    [EndpointName("GetTherapyCardById")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetById(int therapyCardId, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetTherapyCardByIdQuery(therapyCardId), ct);
        
        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(TherapyCardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new therapy card.")]
    [EndpointDescription("Creates a new therapy card with the provided details and returns the created therapy card information.")]
    [EndpointName("CreateTherapyCard")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateTherapyCardRequest request, CancellationToken ct = default)
    {
        var programs = request.Programs.ConvertAll(p => new CreateTherapyCardMedicalProgramCommand(p.MedicalProgramId, p.Duration, p.Notes));

        var result = await sender.Send(new CreateTherapyCardCommand(
            TicketId: request.TicketId,
            DiagnosisText: request.DiagnosisText,
            request.InjuryDate,
            request.InjuryReasons,
            request.InjurySides,
            request.InjuryTypes,
            request.ProgramStartDate,
            request.ProgramEndDate,
            request.TherapyCardType,
            programs,
            request.Notes
        ), ct);

         return result.Match(
            response => CreatedAtRoute(
                routeName: "GetTherapyCardById",
                routeValues: new { version = "1.0", therapyCardId = response.TherapyCardId },
                value: response),
                Problem
        );
    }

    [HttpPut("{therapyCardId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing therapy card.")]
    [EndpointDescription("Updates a therapy card and its associated medical programs.")]
    [EndpointName("UpdateTherapyCard")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Update(int therapyCardId, [FromBody] UpdateTherapyCardRequest request, CancellationToken ct)
    {
        var programs = request.Programs
            .ConvertAll(p => new UpdateTherapyCardMedicalProgramCommand(p.MedicalProgramId, p.Duration, p.Notes));

        var result = await sender.Send(new UpdateTherapyCardCommand(
            TherapyCardId: therapyCardId,
            TicketId: request.TicketId,
            DiagnosisText: request.DiagnosisText,
            request.InjuryDate,
            request.InjuryReasons,
            request.InjurySides,
            request.InjuryTypes,
            request.ProgramStartDate,
            request.ProgramEndDate,
            request.TherapyCardType,
            programs,
            request.Notes
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem);
    }

    [HttpPost("{therapyCardId:int}/renew")]
    [ProducesResponseType(typeof(TherapyCardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Renews an existing therapy card.")]
    [EndpointDescription("Renews a therapy card with updated details and medical programs.")]
    [EndpointName("RenewTherapyCard")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> Renew(int therapyCardId, [FromBody] RenewTherapyCardRequest request, CancellationToken ct)
    {
        var programs = request.Programs
            .ConvertAll(p => new RenewTherapyCardMedicalProgramCommand(p.MedicalProgramId, p.Duration, p.Notes));

        var result = await sender.Send(new RenewTherapyCardCommand(
            therapyCardId,
            request.TicketId,
            request.ProgramStartDate,
            request.ProgramEndDate,
            request.TherapyCardType,
            programs,
            request.Notes
        ), ct);

         return result.Match(
            response => CreatedAtRoute(
                routeName: "GetTherapyCardById",
                routeValues: new { version = "1.0", therapyCardId = response.TherapyCardId },
                value: response),
                Problem
        );
    }

    [HttpPost("{therapyCardId:int}/generate-sessions")]
    [ProducesResponseType(typeof(List<SessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound )]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Generates therapy sessions for a specified therapy card.")]
    [EndpointDescription("Generates a list of therapy sessions associated with the specified therapy card ID.")]
    [EndpointName("GenerateTherapySessions")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> GenerateSessions(int therapyCardId, CancellationToken ct = default)
    {

        var result = await sender.Send(new GenerateSessionsCommand(therapyCardId), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPut("{therapyCardId:int}/sessions/{sessionId:int}/take-session")]
    [ProducesResponseType(typeof(List<SessionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound )]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Take therapy session for a specified therapy card.")]
    [EndpointDescription("Assign therapy session for doctor and mark it as taken for specific therapyCard.")]
    [EndpointName("TakeTherapySession")]
    [MapToApiVersion("1.0")]
    public async Task<IActionResult> TakeSession(int therapyCardId, int sessionId, [FromBody] List<TakeSessionRequest> request)
    {
        var sessionProgramData = request
            .ConvertAll(p => new SessionProgramData(p.DiagnosisProgramId, p.DoctorSectionRoomId));

        var response = await sender.Send(new TakeSessionCommand(therapyCardId, sessionId,sessionProgramData));

        return response.Match(
            _=> NoContent(),
            Problem
        );
    }
}