using System.Data;

using AlatrafClinic.Application.Reports.Interfaces;

using Dapper;

public class DapperReportQueryExecutor : IReportQueryExecutor
{
    private readonly IDbConnection _connection;

    public DapperReportQueryExecutor(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<IEnumerable<IDictionary<string, object>>> ExecuteAsync(
        string sql,
        DynamicParameters parameters)
    {
        var rows = await _connection.QueryAsync(sql, parameters);

        return rows.Select(r => (IDictionary<string, object>)r);
    }
}
