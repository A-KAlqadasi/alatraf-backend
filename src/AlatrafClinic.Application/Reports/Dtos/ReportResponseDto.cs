
public class ReportResponseDto
{
    public List<ReportColumnDto> Columns { get; set; } = new();
    public List<IDictionary<string, object>> Rows { get; set; } = new();
    
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
}


public class ReportColumnDto
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}