namespace AlatrafClinic.Application.Features.Services.Dtos;


public class ServiceDto
{
    public int ServiceId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public string Department { get; set; } = string.Empty;
    public decimal? Price { get; set; } = null;

}