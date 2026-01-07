using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Domain.Reports;

using Dapper;

using Microsoft.Extensions.Logging;

public class ReportSqlBuilder : IReportSqlBuilder
{
    private readonly ISqlDialect _sqlDialect;
    private readonly ILogger<ReportSqlBuilder> _logger;
    private static readonly Regex _validIdentifierRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);
    private static readonly HashSet<string> _validOperators = new(StringComparer.OrdinalIgnoreCase)
    {
        "=", "!=", "<>", ">", "<", ">=", "<=",
        "LIKE", "NOT LIKE", "IN", "NOT IN", "BETWEEN",
        "IS NULL", "IS NOT NULL"
    };

    public ReportSqlBuilder(ISqlDialect sqlDialect, ILogger<ReportSqlBuilder> logger)
    {
        _sqlDialect = sqlDialect;
        _logger = logger;
    }

    public (string DataSql, string CountSql, DynamicParameters Parameters) Build(
        ReportDomain domain, 
        List<ReportField> fields, 
        List<ReportJoin> joins, 
        ReportRequestDto request)
    {
        var parameters = new DynamicParameters();
        var selectedFields = fields.Where(f => request.SelectedFields.Contains(f.FieldKey)).ToList();

        // Build common parts
        var selectClause = BuildSelectClause(selectedFields);
        var fromClause = BuildFromClause(domain, selectedFields, joins);
        var whereClause = BuildWhereClause(request, fields, parameters);
        var orderByClause = BuildOrderByClause(request, fields);
        var paginationClause = BuildPaginationClause(request, parameters);

        // Build data query
        var dataSql = $@"
            {selectClause}
            {fromClause}
            {whereClause}
            {orderByClause}
            {paginationClause}";

        // Build count query (without SELECT columns, ORDER BY, or pagination)
        var countSql = $@"
            SELECT COUNT(*)
            {fromClause}
            {whereClause}";

        _logger.LogDebug("Generated SQL for domain {DomainId}. Data query length: {DataLength}, Count query length: {CountLength}",
            domain.Id, dataSql.Length, countSql.Length);

        return (dataSql.Trim(), countSql.Trim(), parameters);
    }

    private string BuildSelectClause(List<ReportField> selectedFields)
    {
        var columnExpressions = selectedFields.Select(f =>
        {
            var safeTable = SanitizeIdentifier(f.TableName);
            var safeColumn = SanitizeIdentifier(f.ColumnName);
            var safeAlias = SanitizeIdentifier(f.FieldKey);
            
            return $"{safeTable}.{safeColumn} AS [{safeAlias}]";
        });

        return $"SELECT {string.Join(", ", columnExpressions)}";
    }

    private string BuildFromClause(ReportDomain domain, List<ReportField> selectedFields, List<ReportJoin> joins)
    {
        var fromClause = new StringBuilder();
        fromClause.Append($"FROM {SanitizeIdentifier(domain.RootTable)}");

        // Determine required tables
        var requiredTables = selectedFields
            .Select(f => f.TableName)
            .Where(t => t != domain.RootTable)
            .Distinct()
            .ToList();

        // Get joins in correct order (respecting dependencies)
        var orderedJoins = OrderJoinsByDependency(joins, domain.RootTable, requiredTables);

        foreach (var join in orderedJoins)
        {
            var safeFromTable = SanitizeIdentifier(join.FromTable);
            var safeToTable = SanitizeIdentifier(join.ToTable);
            var safeCondition = SanitizeJoinCondition(join.JoinCondition);

            var joinType = join.JoinType.ToUpper() switch
            {
                "INNER" => "INNER JOIN",
                "LEFT" => "LEFT JOIN",
                "RIGHT" => "RIGHT JOIN",
                "FULL" => "FULL OUTER JOIN",
                _ => "INNER JOIN"
            };

            fromClause.Append($" {joinType} {safeToTable} ON {safeCondition}");
        }

        return fromClause.ToString();
    }

    private string BuildWhereClause(ReportRequestDto request, List<ReportField> fields, DynamicParameters parameters)
    {
        if (!request.Filters.Any())
            return string.Empty;

        var whereParts = new List<string>();
        int paramIndex = 0;

        foreach (var filter in request.Filters)
        {
            var field = fields.FirstOrDefault(f => f.FieldKey == filter.FieldKey);
            if (field == null)
                throw new ValidationException($"Filter field '{filter.FieldKey}' not found");

            if (!field.IsFilterable)
                throw new ValidationException($"Field '{filter.FieldKey}' is not filterable");

            if (!_validOperators.Contains(filter.Operator))
                throw new ValidationException($"Invalid operator '{filter.Operator}' for field '{filter.FieldKey}'");

            var safeTable = SanitizeIdentifier(field.TableName);
            var safeColumn = SanitizeIdentifier(field.ColumnName);
            var columnReference = $"{safeTable}.{safeColumn}";

            whereParts.Add(BuildFilterExpression(columnReference, filter, field, parameters, ref paramIndex));
        }

        return $"WHERE {string.Join(" AND ", whereParts)}";
    }

    private string BuildFilterExpression(
    string columnReference, 
    ReportFilterDto filter, 
    ReportField field, // Add this parameter
    DynamicParameters parameters, 
    ref int paramIndex)
    {
        var paramName = $"@p{paramIndex++}";

        switch (filter.Operator.ToUpper())
        {
            case "BETWEEN":
                var array = (JsonElement)filter.Value;
                if (array.ValueKind != JsonValueKind.Array || array.GetArrayLength() != 2)
                    throw new ValidationException("BETWEEN operator requires an array of two values");

                var paramName1 = $"@p{paramIndex++}";
                var paramName2 = $"@p{paramIndex++}";

                // Pass the data type hint for date conversion
                parameters.Add(paramName1, JsonElementHelper.ExtractValue(array[0], field.DataType));
                parameters.Add(paramName2, JsonElementHelper.ExtractValue(array[1], field.DataType));

                return $"{columnReference} BETWEEN {paramName1} AND {paramName2}";

            case "IN":
            case "NOT IN":
                var inArray = (JsonElement)filter.Value;
                if (inArray.ValueKind != JsonValueKind.Array)
                    throw new ValidationException($"{filter.Operator} operator requires an array");

                var inValues = new List<string>();
                for (int i = 0; i < inArray.GetArrayLength(); i++)
                {
                    var param = $"@p{paramIndex++}";
                    inValues.Add(param);
                    parameters.Add(param, JsonElementHelper.ExtractValue(inArray[i], field.DataType));
                }

                return $"{columnReference} {filter.Operator.ToUpper()} ({string.Join(", ", inValues)})";

            case "LIKE":
            case "NOT LIKE":
                var value = JsonElementHelper.ExtractValue((JsonElement)filter.Value, field.DataType);
                parameters.Add(paramName, $"%{value}%");
                return $"{columnReference} {filter.Operator.ToUpper()} {paramName}";

            case "IS NULL":
            case "IS NOT NULL":
                return $"{columnReference} {filter.Operator.ToUpper()}";

            default:
                // Pass the data type hint for date conversion
                parameters.Add(paramName, JsonElementHelper.ExtractValue((JsonElement)filter.Value, field.DataType));
                return $"{columnReference} {filter.Operator} {paramName}";
        }
    }

    private string BuildOrderByClause(ReportRequestDto request, List<ReportField> fields)
    {
        if (!request.SortBy.Any())
            return string.Empty;

        var orderParts = new List<string>();
        
        foreach (var sort in request.SortBy)
        {
            var field = fields.First(f => f.FieldKey == sort.FieldKey);
            var safeTable = SanitizeIdentifier(field.TableName);
            var safeColumn = SanitizeIdentifier(field.ColumnName);
            
            orderParts.Add($"{safeTable}.{safeColumn} {sort.Direction.ToUpper()}");
        }

        return $"ORDER BY {string.Join(", ", orderParts)}";
    }

    private string BuildPaginationClause(ReportRequestDto request, DynamicParameters parameters)
    {
        if (request.PageSize <= 0)
            return string.Empty;

        parameters.Add("@Offset", (request.Page - 1) * request.PageSize);
        parameters.Add("@PageSize", request.PageSize);

        return _sqlDialect.GetPaginationSyntax();
    }

    private List<ReportJoin> OrderJoinsByDependency(List<ReportJoin> joins, string rootTable, List<string> requiredTables)
    {
        var ordered = new List<ReportJoin>();
        var processedTables = new HashSet<string> { rootTable };
        var remainingJoins = new List<ReportJoin>(joins.Where(j => requiredTables.Contains(j.ToTable)));

        while (remainingJoins.Any())
        {
            var joinAdded = false;
            
            for (int i = remainingJoins.Count - 1; i >= 0; i--)
            {
                var join = remainingJoins[i];
                
                if (processedTables.Contains(join.FromTable))
                {
                    ordered.Add(join);
                    processedTables.Add(join.ToTable);
                    remainingJoins.RemoveAt(i);
                    joinAdded = true;
                }
            }

            if (!joinAdded && remainingJoins.Any())
                throw new ReportConfigurationException($"Circular or broken join dependency detected");
        }

        return ordered;
    }

    private string SanitizeIdentifier(string identifier)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            throw new ValidationException("Identifier cannot be empty");

        if (!_validIdentifierRegex.IsMatch(identifier))
            throw new ValidationException($"Invalid identifier format: {identifier}");

        return identifier;
    }

    private string SanitizeJoinCondition(string condition)
    {
        // Simple sanitization - in production, use a proper SQL parser
        var parts = condition.Split(new[] { '=', '!', '<', '>', ' ', '\t', '\n', '\r', '(', ')' }, 
            StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var part in parts)
        {
            if (part.Contains('.'))
            {
                var tableColumn = part.Split('.');
                if (tableColumn.Length == 2)
                {
                    var safeTable = SanitizeIdentifier(tableColumn[0]);
                    var safeColumn = SanitizeIdentifier(tableColumn[1]);
                    condition = condition.Replace(part, $"{safeTable}.{safeColumn}");
                }
            }
        }

        return condition;
    }
}

// SQL Dialect abstraction
public interface ISqlDialect
{
    string GetPaginationSyntax();
}

public class SqlServerDialect : ISqlDialect
{
    public string GetPaginationSyntax() => "OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
}

public class PostgreSqlDialect : ISqlDialect
{
    public string GetPaginationSyntax() => "LIMIT @PageSize OFFSET @Offset";
}

public class MySqlDialect : ISqlDialect
{
    public string GetPaginationSyntax() => "LIMIT @Offset, @PageSize";
}