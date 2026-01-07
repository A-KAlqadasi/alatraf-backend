namespace AlatrafClinic.Application.Reports.Dtos;

public class ReportRequestDto
{
    public int DomainId { get; set; }
    public List<string> SelectedFields { get; set; } = new();
    public List<ReportFilterDto> Filters { get; set; } = new();
}

public class ReportFilterDto
{
    public string FieldKey { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public object Value { get; set; } = default!;
}

