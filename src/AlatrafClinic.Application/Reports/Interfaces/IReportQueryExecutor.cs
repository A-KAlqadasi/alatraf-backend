using Dapper;

namespace AlatrafClinic.Application.Reports.Interfaces;

public interface IReportQueryExecutor
{
    Task<IEnumerable<IDictionary<string, object>>> ExecuteAsync(
        string sql, 
        DynamicParameters parameters,
        CancellationToken cancellationToken = default);
    
    Task<T> ExecuteScalarAsync<T>(
        string sql, 
        DynamicParameters parameters,
        CancellationToken cancellationToken = default);
}