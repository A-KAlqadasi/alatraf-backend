using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Domain.Reports;

using Dapper;

namespace AlatrafClinic.Application.Reports.Interfaces;

public interface IReportSqlBuilder
{
    (string DataSql, string CountSql, DynamicParameters Parameters) Build(
        ReportDomain domain,
        List<ReportField> fields,
        List<ReportJoin> joins,
        ReportRequestDto request);
}