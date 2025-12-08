using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Api.Requests.Patients;

public class PatientFilterRequest
{
   public string? SearchTerm { get; set; }
   public PatientType? PatientType { get; set; }
   public bool? Gender { get; set; }
   public DateTime? BirthDateFrom { get; set; }
   public DateTime? BirthDateTo { get; set; }
   public bool? HasNationalNo {get; set;}
   public string SortColumn { get; set; } = "fullname";
   public string SortDirection { get; set; } = "asc";
}