namespace AlatrafClinic.Application.Reports.CountableReports.Dtos;


public class ReportBasedOnInjuryDto
{
    public string InjuryDetail { get; set; } = null!;
    public int PatientCount { get; set; }
    public int ServicesCount { get; set; }
    public decimal TotalServicesPrice { get; set; }
    public string DepartmentName { get; set; } = null!;
}