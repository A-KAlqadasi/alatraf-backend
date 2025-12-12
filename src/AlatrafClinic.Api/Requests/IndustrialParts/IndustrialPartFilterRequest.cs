using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.IndustrialParts;

public class IndustrialPartFilterRequest
{
    public string? SearchTerm { get; set; }
    
    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "SortColumn contains invalid characters.")]
    public string SortColumn { get; set; } = "name";

    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; set; } = "asc";
}