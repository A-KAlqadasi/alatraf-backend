using System.ComponentModel.DataAnnotations;

namespace AlatrafClinic.Api.Requests.Appointments;

public sealed class RescheduleAppointmentRequest
{
    [Required(ErrorMessage = "NewAttendDate is required")]
    [DataType(DataType.Date)]
    public DateOnly NewAttendDate { get; init; }
}