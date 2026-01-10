using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Application.Reports.Dtos;


public class ReportRequestDto
{
    public int DomainId { get; set; }
    public List<string> SelectedFields { get; set; } = new();
    public List<ReportFilterDto> Filters { get; set; } = new();
    
    public int Page { get; set; } = 1;

    public int PageSize { get; set; }
    public int MaxRows { get; set; } = 10000;
    public List<ReportSortDto> SortBy { get; set; } = new();
}

public class ReportSortDto
{
    public string FieldKey { get; set; } = string.Empty;
    public string Direction { get; set; } = "ASC"; // ASC or DESC
}

public class ReportFilterDto
{
    public string FieldKey { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public object Value { get; set; } = default!;
}
