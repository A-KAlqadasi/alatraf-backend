using System.Data;

using AlatrafClinic.Application.Reports.Exceptions;
using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Infrastructure.Reports.Helpers;

using Dapper;

using Microsoft.Extensions.Logging;


public class DapperReportQueryExecutor : IReportQueryExecutor
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DapperReportQueryExecutor> _logger;

    public DapperReportQueryExecutor(
        IDbConnectionFactory connectionFactory,
        ILogger<DapperReportQueryExecutor> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<IDictionary<string, object>>> ExecuteAsync(
        string sql, 
        DynamicParameters parameters,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        try
        {
            _logger.LogDebug("Executing report query. Length: {SqlLength}, Parameters: {ParamCount}", 
                sql.Length, parameters.ParameterNames.Count());

            var command = new CommandDefinition(
                sql,
                parameters,
                commandTimeout: 300, // 5 minutes timeout
                cancellationToken: cancellationToken);

            var results = await connection.QueryAsync(command);
            
            _logger.LogDebug("Query returned {RowCount} rows", results.Count());
            
            return results.Select(row => (IDictionary<string, object>)row);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing report query");
            throw new ReportExecutionException("Failed to execute report query", ex);
        }
    }

    public async Task<T> ExecuteScalarAsync<T>(
        string sql, 
        DynamicParameters parameters,
        CancellationToken cancellationToken = default)
    {
        using var connection = await _connectionFactory.CreateConnectionAsync();
        
        try
        {
            var command = new CommandDefinition(
                sql,
                parameters,
                commandTimeout: 60, // 1 minute for count queries
                cancellationToken: cancellationToken);

            return await connection.ExecuteScalarAsync<T>(command);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing scalar query");
            throw new ReportExecutionException("Failed to execute count query", ex);
        }
    }
}