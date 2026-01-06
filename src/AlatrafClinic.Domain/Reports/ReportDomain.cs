namespace AlatrafClinic.Domain.Reports;

public class ReportDomain
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RootTable { get; set; } = string.Empty;

    public List<ReportField> Fields { get; set; } = new();
    public List<ReportJoin> Joins { get; set; } = new();
}

public class ReportField
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public string FieldKey { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public bool IsFilterable { get; set; }
}

public class ReportJoin
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public string FromTable { get; set; } = string.Empty;
    public string ToTable { get; set; } = string.Empty;
    public string JoinType { get; set; } = string.Empty;
    public string JoinCondition { get; set; } = string.Empty;
}
