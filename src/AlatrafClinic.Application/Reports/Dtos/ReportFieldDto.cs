namespace AlatrafClinic.Application.Reports.Dtos;

public class ReportFieldDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsFilterable { get; set; }
    public bool IsSortable { get; set; }
    public int DisplayOrder { get; set; }
    public string TableName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
}