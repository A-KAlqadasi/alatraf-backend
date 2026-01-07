using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Domain.Reports;
using AlatrafClinic.Infrastructure.Reports.Helpers;

using Dapper;

using Microsoft.Extensions.Logging;

public class ReportMetadataRepository : IReportMetadataRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<ReportMetadataRepository> _logger;

    public ReportMetadataRepository(
        IDbConnectionFactory connectionFactory,
        ILogger<ReportMetadataRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<ReportDomainWithRelations> GetDomainWithRelationsAsync(int domainId)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        try
        {
            const string sql = @"
                -- Get Domain with new properties
                SELECT 
                    d.Id, 
                    d.Name, 
                    d.RootTable,
                    d.IsActive,
                    d.CreatedAt,
                    d.UpdatedAt
                FROM ReportDomains d 
                WHERE d.Id = @DomainId AND d.IsActive = 1;
                
                -- Get Fields with new properties
                SELECT 
                    f.Id,
                    f.DomainId,
                    f.FieldKey,
                    f.DisplayName,
                    f.TableName,
                    f.ColumnName,
                    f.DataType,
                    f.IsFilterable,
                    f.IsSortable,        -- New
                    f.IsActive,         -- New
                    f.DisplayOrder,     -- New
                    f.DefaultOrder,     -- New
                    f.CreatedAt,        -- New
                    f.UpdatedAt         -- New
                FROM ReportFields f 
                WHERE f.DomainId = @DomainId AND f.IsActive = 1
                ORDER BY f.DisplayOrder;
                
                -- Get Joins with new properties
                SELECT 
                    j.Id,
                    j.DomainId,
                    j.FromTable,
                    j.ToTable,
                    j.JoinType,
                    j.JoinCondition,
                    j.IsActive,        -- New
                    j.IsRequired,      -- New
                    j.JoinOrder,       -- New
                    j.CreatedAt,       -- New
                    j.UpdatedAt        -- New
                FROM ReportJoins j 
                WHERE j.DomainId = @DomainId AND j.IsActive = 1
                ORDER BY j.JoinOrder;";

            using var multi = await connection.QueryMultipleAsync(sql, new { DomainId = domainId });
            
            var domain = await multi.ReadSingleOrDefaultAsync<ReportDomain>();
            if (domain == null)
                throw new ReportConfigurationException($"Report domain {domainId} not found or inactive");
            
            var fields = (await multi.ReadAsync<ReportField>()).ToList();
            var joins = (await multi.ReadAsync<ReportJoin>()).ToList();

            if (!fields.Any())
                throw new ReportConfigurationException($"No fields configured for domain {domainId}");

            // Log with Serilog structured logging
            _logger.LogInformation(
                "Loaded domain {DomainId} ({DomainName}) with {FieldCount} fields and {JoinCount} joins", 
                domainId, domain.Name, fields.Count, joins.Count);

            return new ReportDomainWithRelations
            {
                Domain = domain,
                Fields = fields,
                Joins = joins,
                CachedAt = DateTimeOffset.UtcNow
            };
        }
        catch (Exception ex) when (ex is not ReportConfigurationException)
        {
            _logger.LogError(ex, "Error loading report metadata for domain {DomainId}", domainId);
            throw new ReportConfigurationException($"Failed to load report configuration: {ex.Message}", ex);
        }
    }
}

public class ReportConfigurationException : Exception
{
    public ReportConfigurationException(string message) : base(message) { }
    public ReportConfigurationException(string message, Exception inner) : base(message, inner) { }
}