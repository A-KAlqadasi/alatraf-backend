using System.ComponentModel.DataAnnotations;

using AlatrafClinic.Domain.Services.Enums;

namespace AlatrafClinic.Api.Requests.Appointments;

public sealed class ChangeAppointmentStatusRequest
{
    [Required(ErrorMessage = "Status is required")]
    public AppointmentStatus Status { get; init; }

}