public class ReportResponseDto
{
    public List<ReportColumnDto> Columns { get; set; } = new();
    public List<IDictionary<string, object>> Rows { get; set; } = new();
}

public class ReportColumnDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}