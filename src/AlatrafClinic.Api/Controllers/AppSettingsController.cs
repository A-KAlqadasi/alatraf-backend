using AlatrafClinic.Api.Requests.AppSettings;
using AlatrafClinic.Application.Features.Settings.Commands;
using AlatrafClinic.Application.Features.Settings.Commands.UpdateAppSetting;
using AlatrafClinic.Application.Features.Settings.Dtos;
using AlatrafClinic.Application.Features.Settings.Queries.GetAppSettingByKey;
using AlatrafClinic.Application.Features.Settings.Queries.GetAppSettings;

using Asp.Versioning;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[Route("api/v{version:apiVersion}/settings")]
[ApiVersion("1.0")]
public sealed class SettingsController(ISender sender) : ApiController
{
    [HttpGet]
    [ProducesResponseType(typeof(List<AppSettingDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves all application settings.")]
    [EndpointDescription("Returns the full list of application settings (key/value pairs with optional descriptions).")]
    [EndpointName("GetAllAppSettings")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAllAppSettingsQuery(), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpGet("{key}", Name = "GetAppSettingByKey")]
    [ProducesResponseType(typeof(AppSettingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Retrieves an application setting by key.")]
    [EndpointDescription("Fetches a single application setting using its unique key.")]
    [EndpointName("GetAppSettingByKey")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> GetByKey(string key, CancellationToken ct = default)
    {
        var result = await sender.Send(new GetAppSettingByKeyQuery(key), ct);

        return result.Match(
            response => Ok(response),
            Problem
        );
    }

    [HttpPost]
    [ProducesResponseType(typeof(AppSettingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Creates a new application setting.")]
    [EndpointDescription("Creates a new application setting using the provided key/value and optional description.")]
    [EndpointName("CreateAppSetting")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Create([FromBody] CreateAppSettingRequest request, CancellationToken ct = default)
    {
        var result = await sender.Send(new CreateAppSettingCommand(
            request.Key,
            request.Value,
            request.Description
        ), ct);

        return result.Match(
            response => CreatedAtRoute(
                routeName: "GetAppSettingByKey",
                routeValues: new { version = "1.0", key = response.Key },
                value: response),
            Problem
        );
    }

    [HttpPut("{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("Updates an existing application setting.")]
    [EndpointDescription("Updates the value and description of an existing application setting using its unique key.")]
    [EndpointName("UpdateAppSetting")]
    [ApiVersion("1.0")]
    public async Task<IActionResult> Update(
        string key,
        [FromBody] UpdateAppSettingRequest request,
        CancellationToken ct = default)
    {
        var result = await sender.Send(new UpdateAppSettingCommand(
            key,
            request.Value,
            request.Description
        ), ct);

        return result.Match(
            _ => NoContent(),
            Problem
        );
    }
}