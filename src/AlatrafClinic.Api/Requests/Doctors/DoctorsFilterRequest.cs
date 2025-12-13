using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Doctors;

public class DoctorsFilterRequest
{
    public int? DepartmentId { get; set; }
    public int? SectionId {get; set; }
    public int? RoomId {get; set;}
    public string? Search { get; set; }
    public string? Specialization {get; set; }
    public bool? HasActiveAssignment { get; set; }
    public string SortBy = "assigndate";
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDir = "desc";
}