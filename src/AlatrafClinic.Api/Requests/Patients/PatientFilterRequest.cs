using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Api.Requests.Patients;

public class PatientFilterRequest
{
    public string? SearchTerm { get; set; }
    public PatientType? PatientType { get; set; }
    public bool? Gender { get; set; }
    public DateOnly? BirthDateFrom { get; set; }
    public DateOnly? BirthDateTo { get; set; }
    public bool? HasNationalNo {get; set;}

    [RegularExpression("^[a-zA-Z0-9_]+$", ErrorMessage = "SortColumn contains invalid characters.")]
    public string SortColumn { get; set; } = "fullname";
    
    [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
    public string SortDirection { get; set; } = "asc";
}