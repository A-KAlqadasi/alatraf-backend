using System.Diagnostics;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Application.Reports.Exceptions;
using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Domain.Reports;

using ClosedXML.Excel;

using FluentValidation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Infrastructure.Reports;

public class ReportService : IReportService
{
    private readonly IReportMetadataRepository _metadataRepo;
    private readonly IReportSqlBuilder _sqlBuilder;
    private readonly IReportQueryExecutor _executor;
    private readonly HybridCache _cache;
    private readonly ILogger<ReportService> _logger;
    private readonly IValidator<ReportRequestDto> _validator;
    private readonly IUser _user;
    private readonly IAppDbContext _context;

    public ReportService(
        IReportMetadataRepository metadataRepo,
        IReportSqlBuilder sqlBuilder,
        IReportQueryExecutor executor,
        HybridCache cache,
        ILogger<ReportService> logger,
        IValidator<ReportRequestDto> validator,
        IUser user,
        IAppDbContext context)
    {
        _metadataRepo = metadataRepo;
        _sqlBuilder = sqlBuilder;
        _executor = executor;
        _cache = cache;
        _logger = logger;
        _validator = validator;
        _user = user;
        _context = context;
    }

    public async Task<ReportResponseDto> RunAsync(ReportRequestDto request)
    {
        var stopwatch = Stopwatch.StartNew();
        var correlationId = Guid.NewGuid();
        
                using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId,
            ["DomainId"] = request.DomainId,
            ["UserId"] = _user.Id ?? "anonymous" // Replace with actual user ID from context
        }))
        {
            try
            {
                _logger.LogInformation(
                    "Starting report executio {CorrelationId} for domain {DomainId} with {FieldCount} fields, by user {UserId}",
                    correlationId, request.DomainId, request.SelectedFields.Count, _user.Id ?? "anonymous");

                // Validate request
                var validationResult = await _validator.ValidateAsync(request);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    throw new ValidationException($"Invalid report request: {errors}");
                }

                // Get domain with caching
                var cacheKey = $"report:domain:{request.DomainId}";
                var domainWithRelations = await _cache.GetOrCreateAsync(
                    cacheKey,
                    async (cancel) =>
                    {
                        _logger.LogDebug("Cache miss for domain {DomainId}, loading from database", request.DomainId);
                        return await _metadataRepo.GetDomainWithRelationsAsync(request.DomainId);
                    },
                    new HybridCacheEntryOptions
                    {
                        Expiration = TimeSpan.FromMinutes(30),
                        LocalCacheExpiration = TimeSpan.FromMinutes(5)
                    });

                var domain = domainWithRelations.Domain;
                var fields = domainWithRelations.Fields;
                var joins = domainWithRelations.Joins;

                // Validate selected fields exist
                var selectedFields = ValidateAndGetSelectedFields(fields, request.SelectedFields);
                
                // Validate sort fields exist and are sortable
                ValidateSortFields(fields, request.SortBy);

                // Build SQL with pagination
                var (dataSql, countSql, parameters) = 
                    _sqlBuilder.Build(domain, fields, joins, request);
                
                var countResult = await _executor.ExecuteScalarAsync<int>(countSql, parameters);

                // Apply MaxRows limit
                if (countResult > request.MaxRows)
                {
                    _logger.LogWarning(
                        "Report {CorrelationId} exceeded MaxRows limit: {TotalCount} > {MaxRows}",
                        correlationId, countResult, request.MaxRows);
                    throw new ReportLimitExceededException(
                        $"Report would return {countResult} rows, which exceeds the maximum allowed ({request.MaxRows})");
                }
                
                // Execute main query
                var rows = await _executor.ExecuteAsync(dataSql, parameters);

                // Prepare response
                var response = new ReportResponseDto
                {
                    Columns = selectedFields.Select(f => new ReportColumnDto
                    {
                        Key = f.FieldKey,
                        Label = f.DisplayName,
                        Type = f.DataType
                    }).ToList(),
                    Rows = rows.ToList(),
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalCount = countResult
                };

                stopwatch.Stop();
                
                _logger.LogInformation(
                    "Report execution {CorrelationId} completed in {ElapsedMs}ms. Rows: {RowCount}, Total: {TotalCount}",
                    correlationId, stopwatch.ElapsedMilliseconds, response.Rows.Count, countResult);

                return response;
            }
            catch (Exception ex) when (ex is not ValidationException and not ReportLimitExceededException)
            {
                _logger.LogError(ex, "Report execution {CorrelationId} failed", correlationId);
                throw new ReportExecutionException(
                    $"Failed to execute report: {ex.Message}. Correlation ID: {correlationId}", ex);
            }
    }
}
    private List<ReportField> ValidateAndGetSelectedFields(List<ReportField> allFields, List<string> selectedFieldKeys)
    {
        if (!selectedFieldKeys.Any())
            throw new ValidationException("At least one field must be selected");

        var selectedFields = new List<ReportField>();
        var invalidFields = new List<string>();

        foreach (var fieldKey in selectedFieldKeys)
        {
            var field = allFields.FirstOrDefault(f => f.FieldKey == fieldKey);
            if (field == null)
                invalidFields.Add(fieldKey);
            else
                selectedFields.Add(field);
        }

        if (invalidFields.Any())
            throw new ValidationException($"Invalid field keys: {string.Join(", ", invalidFields)}");

        return selectedFields;
    }

    private void ValidateSortFields(List<ReportField> allFields, List<ReportSortDto> sortFields)
    {
        foreach (var sort in sortFields)
        {
            var field = allFields.FirstOrDefault(f => f.FieldKey == sort.FieldKey);
            if (field == null)
                throw new ValidationException($"Sort field '{sort.FieldKey}' not found");

            if (!field.IsSortable)
                throw new ValidationException($"Field '{sort.FieldKey}' is not sortable");

            if (sort.Direction.ToUpper() != "ASC" && sort.Direction.ToUpper() != "DESC")
                throw new ValidationException($"Invalid sort direction '{sort.Direction}' for field '{sort.FieldKey}'");
        }
    }

        public async Task<IEnumerable<ReportDomainDto>> GetAvailableDomainsAsync()
    {
        try
        {
            var domains = await _context.ReportDomains
                .AsNoTracking()
                .Where(d => d.IsActive)
                .OrderBy(d => d.Name)
                .Select(d => new ReportDomainDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    RootTable = d.RootTable,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync();

            _logger.LogDebug("Retrieved {DomainCount} active report domains", domains.Count);
            return domains;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving report domains");
            throw;
        }
    }

    public async Task<IEnumerable<ReportFieldDto>> GetDomainFieldsAsync(int domainId)
    {
        try
        {
            // Get cached domain with relations
            var cacheKey = $"report:domain:{domainId}";
            var domainWithRelations = await _cache.GetOrCreateAsync(
                cacheKey,
                async (cancel) => await _metadataRepo.GetDomainWithRelationsAsync(domainId),
                new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(30) });

            var fields = domainWithRelations.Fields
                .Where(f => f.IsActive)
                .OrderBy(f => f.DisplayOrder)
                .Select(f => new ReportFieldDto
                {
                    Key = f.FieldKey,
                    Label = f.DisplayName,
                    Type = f.DataType,
                    IsFilterable = f.IsFilterable,
                    IsSortable = f.IsSortable,
                    DisplayOrder = f.DisplayOrder,
                    TableName = f.TableName,
                    ColumnName = f.ColumnName
                });

            _logger.LogDebug("Retrieved {FieldCount} fields for domain {DomainId}", 
                fields.Count(), domainId);
                
            return fields;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving fields for domain {DomainId}", domainId);
            throw;
        }
    }

    public async Task<int> GetRowCountAsync(ReportRequestDto request)
    {
        var domainWithRelations = await _metadataRepo.GetDomainWithRelationsAsync(request.DomainId);
        
        var (_, countSql, parameters) = _sqlBuilder.Build(
            domainWithRelations.Domain,
            domainWithRelations.Fields,
            domainWithRelations.Joins,
            request);
        
        // Execute count query only
        return await _executor.ExecuteScalarAsync<int>(countSql, parameters);
    }

    
}