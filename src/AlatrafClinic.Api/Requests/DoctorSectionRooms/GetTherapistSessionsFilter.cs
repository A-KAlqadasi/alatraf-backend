namespace AlatrafClinic.Api.Requests.DoctorSectionRooms;

public class GetTherapistSessionsFilter
{
    public DateOnly? Date { get; set; }
    public string? PatientName { get; set; }
    public int? TherapyCardId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
