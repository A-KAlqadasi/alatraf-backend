using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.People;

public class CreatePersonRequest
{
    [Required(ErrorMessage = "Fullname is required.")]
    public string Fullname { get; set; } = default!;

    [Required(ErrorMessage ="Birthdate is required")]
    public DateOnly Birthdate { get; set; }
    
    [Required(ErrorMessage = "Phone number is required")]
    [RegularExpression(@"^(77|78|73|71|70)\d{7}$", ErrorMessage = "Phone number must start with 77, 78, 73, 71, or 70 and be 9 digits long.")]
    public string Phone { get; set; } = default!;

    [Required(ErrorMessage = "National number is required")]
    public string NationalNo {get; set; } = default!;
    [Required(ErrorMessage = "Address is required")]
    public string Address { get ;set; } = default!;
    [Required(ErrorMessage = "Gender is required")]
    public bool Gender {get; set; }
}