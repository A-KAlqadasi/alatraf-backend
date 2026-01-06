using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Application.Reports.Interfaces;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/reports")]
[ApiVersion("1.0")]
public class ReportsController : ApiController
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpPost("run")]
    public async Task<IActionResult> Run([FromBody] ReportRequestDto request)
        => Ok(await _service.RunAsync(request));
}