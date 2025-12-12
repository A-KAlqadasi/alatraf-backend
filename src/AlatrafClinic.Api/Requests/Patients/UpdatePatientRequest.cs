using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Patients.Enums;

namespace AlatrafClinic.Api.Requests.Patients;

public class UpdatePatientRequest
{
    [Required(ErrorMessage = "Fullname is required.")]
    public string Fullname { get; set; } = default!;

    [Required(ErrorMessage ="Birthdate is required")]
    public DateOnly Birthdate { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(77|78|73|71)\d{7}$", ErrorMessage = "Phone number must start with 77, 78, 73, or 71 and be 9 digits long.")]
    public string Phone { get; set; } = default!;
    public string? NationalNo {get; set; }
    [Required(ErrorMessage = "Address is required")]
    public string Address { get ;set; } = default!;
    [Required(ErrorMessage = "Gender is required")]
    public bool Gender {get; set; }
    [Required(ErrorMessage = "Patient Type is required")]
    public PatientType PatientType {get; set; }
}