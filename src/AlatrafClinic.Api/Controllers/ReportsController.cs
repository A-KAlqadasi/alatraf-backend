using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Application.Reports.Exceptions;
using AlatrafClinic.Application.Reports.Interfaces;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

namespace AlatrafClinic.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/reports")]
[ApiVersion("1.0")]
public class ReportsController : ApiController
{
    private readonly IReportService _reportService;
    private readonly IReportExportService _reportExportService;
    private readonly ILogger<ReportsController> _logger;

    public ReportsController(
        IReportService reportService,
        IReportExportService reportExportService,
        ILogger<ReportsController> logger)
    {
        _reportService = reportService;
        _reportExportService = reportExportService;
        _logger = logger;
    }

    /// <summary>
    /// Run a report based on the provided criteria
    /// </summary>
    /// <param name="request">Report request parameters</param>
    /// <returns>Report data with columns and rows</returns>
    [HttpPost("run")]
    [ProducesResponseType(typeof(ReportResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RunReport([FromBody] ReportRequestDto request)
    {
        try
        {
            _logger.LogInformation(
                "Starting report execution for domain {DomainId} by user {UserId}",
                request.DomainId, GetCurrentUserId());

            var result = await _reportService.RunAsync(request);

            _logger.LogInformation(
                "Report completed successfully. Domain: {DomainId}, Rows: {RowCount}",
                request.DomainId, result.Rows.Count);

            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for report request");
            return BadRequest(new
            {
                Error = "Invalid report request",
                Details = ex.Message,
                CorrelationId = GetCorrelationId()
            });
        }
        catch (ReportConfigurationException ex)
        {
            _logger.LogWarning(ex, "Report configuration error");
            return BadRequest(new
            {
                Error = "Report configuration error",
                Details = ex.Message,
                CorrelationId = GetCorrelationId()
            });
        }
        catch (ReportLimitExceededException ex)
        {
            _logger.LogWarning(ex, "Report limit exceeded");
            return BadRequest(new
            {
                Error = "Report limit exceeded",
                Details = ex.Message,
                MaxRows = request.MaxRows,
                CorrelationId = GetCorrelationId()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during report execution");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Error = "An unexpected error occurred",
                Details = ex.Message,
                CorrelationId = GetCorrelationId()
            });
        }
    }

    /// <summary>
    /// Get available report domains (templates)
    /// </summary>
    /// <returns>List of report domains</returns>
    [HttpGet("domains")]
    [ProducesResponseType(typeof(IEnumerable<ReportDomainDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDomains()
    {
        try
        {
            // You'll need to create this method in your service/repository
            var domains = await _reportService.GetAvailableDomainsAsync();
            
            return Ok(domains);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching report domains");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Error = "Failed to fetch report domains",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Get fields for a specific report domain
    /// </summary>
    /// <param name="domainId">Report domain ID</param>
    /// <returns>List of available fields for the domain</returns>
    [HttpGet("domains/{domainId}/fields")]
    [ProducesResponseType(typeof(IEnumerable<ReportFieldDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDomainFields(int domainId)
    {
        try
        {
            // You'll need to create this method in your service/repository
            var fields = await _reportService.GetDomainFieldsAsync(domainId);
            
            if (fields == null || !fields.Any())
                return NotFound(new { Message = $"No fields found for domain {domainId}" });

            return Ok(fields);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching domain fields for domain {DomainId}", domainId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Error = "Failed to fetch domain fields",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Run a report with simplified parameters (for testing)
    /// </summary>
    /// <param name="domainId">Report domain ID</param>
    /// <param name="fields">Comma-separated list of field keys</param>
    /// <returns>Report data</returns>
    [HttpGet("quick-run/{domainId}")]
    [ProducesResponseType(typeof(ReportResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> QuickRun(
        int domainId,
        [FromQuery] string fields = "",
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var request = new ReportRequestDto
            {
                DomainId = domainId,
                SelectedFields = fields.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList(),
                Page = page,
                PageSize = pageSize,
                Filters = new List<ReportFilterDto>()
            };

            var result = await _reportService.RunAsync(request);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in quick run for domain {DomainId}", domainId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                Error = "Quick run failed",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Export report to Excel
    /// </summary>
    /// <param name="request">Report request parameters</param>
    /// <returns>Excel file</returns>
    [HttpPost("export/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportToExcel([FromBody] ReportRequestDto request)
    {
        try
        {
            // You'll need to implement export functionality
            var excelBytes = await _reportExportService.ExportToExcelAsync(await _reportService.RunAsync(request));
            
            var fileName = $"report_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xlsx";
            
            return File(excelBytes, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting report to Excel");
            return BadRequest(new
            {
                Error = "Export failed",
                Details = ex.Message
            });
        }
    }

    /// <summary>
    /// Test endpoint to verify report service is working
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult HealthCheck()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Service = "ReportService",
            Version = "1.0.0"
        });
    }

    #region Helper Methods

    private string GetCurrentUserId()
    {
        // Replace with your actual user ID extraction logic
        // This depends on your authentication setup
        return User?.Identity?.Name ?? "anonymous";
    }

    private string? GetCorrelationId()
    {
        // Get correlation ID from request headers or create new
        if (HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            return correlationId;
        
        return Guid.NewGuid().ToString();
    }

    #endregion

}