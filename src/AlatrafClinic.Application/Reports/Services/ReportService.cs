using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Application.Reports.Interfaces;

namespace AlatrafClinic.Application.Reports.Services;

public class ReportService : IReportService
{
    private readonly IReportMetadataRepository _repo;
    private readonly IReportSqlBuilder _sqlBuilder;
    private readonly IReportQueryExecutor _executor;

    public ReportService(
        IReportMetadataRepository repo,
        IReportSqlBuilder sqlBuilder,
        IReportQueryExecutor executor)
    {
        _repo = repo;
        _sqlBuilder = sqlBuilder;
        _executor = executor;
    }

    public async Task<ReportResponseDto> RunAsync(ReportRequestDto request)
    {
        var domain = await _repo.GetDomainAsync(request.DomainId);
        var fields = await _repo.GetFieldsAsync(request.DomainId);
        var joins = await _repo.GetJoinsAsync(request.DomainId);

        var selectedFields = fields
            .Where(f => request.SelectedFields.Contains(f.FieldKey))
            .ToList();

        var (sql, parameters) =
            _sqlBuilder.Build(domain, fields, joins, request);

        var rows = await _executor.ExecuteAsync(sql, parameters);

        return new ReportResponseDto
        {
            Columns = selectedFields.Select(f => new ReportColumnDto
            {
                Key = f.FieldKey,
                Label = f.DisplayName,
                Type = f.DataType
            }).ToList(),

            Rows = rows.ToList()
        };
    }

}
