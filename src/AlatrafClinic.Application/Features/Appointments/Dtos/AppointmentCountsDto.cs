namespace AlatrafClinic.Application.Features.Appointments.Dtos;

public class AppointmentCountsDto
{
    public DateTime Date { get; set; }
    public int TotalCount { get; set; }
    public int NormalCount { get; set; }
    public int WoundedCount { get; set; }
}