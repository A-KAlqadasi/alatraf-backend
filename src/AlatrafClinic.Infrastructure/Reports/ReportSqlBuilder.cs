using System.Text;
using System.Text.Json;

using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Domain.Reports;

using Dapper;

public class ReportSqlBuilder : IReportSqlBuilder
{
    public (string Sql, DynamicParameters Parameters) Build(ReportDomain domain, List<ReportField> fields, List<ReportJoin> joins, ReportRequestDto request)
    {
         var selectedFields = fields
            .Where(f => request.SelectedFields.Contains(f.FieldKey))
            .ToList();

        if (!selectedFields.Any())
            throw new Exception("No fields selected");

        var sql = new StringBuilder();
        var parameters = new DynamicParameters();

        // -------- SELECT --------
        sql.Append("SELECT ");
        sql.Append(string.Join(", ",
            selectedFields.Select(f =>
                $"{f.TableName}.{f.ColumnName} AS {f.FieldKey}"
            )));

        // -------- FROM --------
        sql.Append($" FROM {domain.RootTable}");

        // -------- JOINS --------
        var requiredTables = selectedFields
            .Select(f => f.TableName)
            .Where(t => t != domain.RootTable)
            .Distinct();

        foreach (var table in requiredTables)
        {
            var join = joins.First(j => j.ToTable == table);
            sql.Append($" {join.JoinType} JOIN {join.ToTable} ON {join.JoinCondition}");
        }

        // -------- WHERE --------
        if (request.Filters.Any())
        {
            sql.Append(" WHERE ");
            var whereParts = new List<string>();
            int paramIndex = 0;

            foreach (var filter in request.Filters)
            {
                var field = fields.First(f => f.FieldKey == filter.FieldKey);

                switch (filter.Operator.ToLower())
                {
                    case "between":
                    {
                        var array = (JsonElement)filter.Value;

                        string p1 = $"@p{paramIndex++}";
                        string p2 = $"@p{paramIndex++}";

                        whereParts.Add(
                            $"{field.TableName}.{field.ColumnName} BETWEEN {p1} AND {p2}");

                        parameters.Add(p1, JsonElementHelper.ExtractValue(array[0]));
                        parameters.Add(p2, JsonElementHelper.ExtractValue(array[1]));
                        break;
                    }

                    default:
                    {
                        string p = $"@p{paramIndex++}";

                        whereParts.Add(
                            $"{field.TableName}.{field.ColumnName} {filter.Operator} {p}");

                        var json = (JsonElement)filter.Value;
                        parameters.Add(p, JsonElementHelper.ExtractValue(json));
                        break;
                    }
                }
            }


            sql.Append(string.Join(" AND ", whereParts));
        }

        return (sql.ToString(), parameters);
    }
}
