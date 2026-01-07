namespace AlatrafClinic.Domain.Reports;

public class ReportDomain
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RootTable { get; set; } = string.Empty;
    
    // New properties for enhanced functionality
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

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
    
    // New properties for enhanced functionality
    public bool IsSortable { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;
    public int? DefaultOrder { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property
    public ReportDomain Domain { get; set; } = null!;
}

public class ReportJoin
{
    public int Id { get; set; }
    public int DomainId { get; set; }
    public string FromTable { get; set; } = string.Empty;
    public string ToTable { get; set; } = string.Empty;
    public string JoinType { get; set; } = string.Empty;
    public string JoinCondition { get; set; } = string.Empty;
    
    // New properties for enhanced functionality
    public bool IsActive { get; set; } = true;
    public bool IsRequired { get; set; } = false;
    public int JoinOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation property
    public ReportDomain Domain { get; set; } = null!;
}

// This class is NOT an EF entity - it's for caching
public class ReportDomainWithRelations
{
    public ReportDomain Domain { get; set; } = null!;
    public List<ReportField> Fields { get; set; } = new();
    public List<ReportJoin> Joins { get; set; } = new();
    public DateTimeOffset CachedAt { get; set; }
}
// Add validation enums
public enum FilterOperator
{
    Equals,
    NotEquals,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    Contains,
    StartsWith,
    EndsWith,
    In,
    Between,
    IsNull,
    IsNotNull
}

public enum JoinType
{
    Inner,
    Left,
    Right,
    Full
}

public enum SortDirection
{
    Ascending,
    Descending
}