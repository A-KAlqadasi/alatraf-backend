using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Doctors;

public class CreateDoctorRequest
{
    [Required(ErrorMessage = "Fullname is required.")]
    public string Fullname { get; set; } = default!;

    [Required(ErrorMessage ="Birthdate is required")]
    public DateOnly Birthdate { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(77|78|73|71)\d{7}$", ErrorMessage = "Phone number must start with 77, 78, 73, or 71 and be 9 digits long.")]
    public string Phone { get; set; } = default!;

    [Required(ErrorMessage = "National number is required")]
    public string NationalNo {get; set; } = default!;
    [Required(ErrorMessage = "Address Id is required")]
    public int AddressId { get ;set; } = default!;
    [Required(ErrorMessage = "Gender is required")]
    public bool Gender {get; set; }

    [Required(ErrorMessage = "Specialization is required")]
    public string Specialization { get; set; } = default!;

    [Required]
    public int DepartmentId {get; set;}
}
