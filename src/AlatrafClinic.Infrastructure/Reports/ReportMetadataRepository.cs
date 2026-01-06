using System.Data;

using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Domain.Reports;

using Dapper;

public class ReportMetadataRepository : IReportMetadataRepository
{
    private readonly IDbConnection _connection;

    public ReportMetadataRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<ReportDomain> GetDomainAsync(int domainId)
        => await _connection.QuerySingleAsync<ReportDomain>(
            "SELECT Id, RootTable FROM ReportDomains WHERE Id = @Id",
            new { Id = domainId });

    public async Task<List<ReportField>> GetFieldsAsync(int domainId)
        => (await _connection.QueryAsync<ReportField>(
            "SELECT FieldKey, DisplayName, TableName, ColumnName, DataType, IsFilterable FROM ReportFields WHERE DomainId = @Id",
            new { Id = domainId })).ToList();

    public async Task<List<ReportJoin>> GetJoinsAsync(int domainId)
        => (await _connection.QueryAsync<ReportJoin>(
            "SELECT FromTable, ToTable, JoinType, JoinCondition FROM ReportJoins WHERE DomainId = @Id",
            new { Id = domainId })).ToList();
}
