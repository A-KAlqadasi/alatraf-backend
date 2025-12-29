namespace AlatrafClinic.Api.Requests.DoctorSectionRooms;

public class GetTechnicianIndustrialPartsFilter
{
    public DateOnly? date { get; set; }
    public int? repairCardId { get; set; }
    public string? patientName { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}