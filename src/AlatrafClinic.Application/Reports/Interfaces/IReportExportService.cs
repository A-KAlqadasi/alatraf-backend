namespace AlatrafClinic.Application.Reports.Interfaces;

public interface IReportExportService
{
    Task<byte[]> ExportToExcelAsync(ReportResponseDto reportResult);
    Task<byte[]> ExportToCsvAsync(ReportResponseDto reportResult);
    Task<byte[]> ExportToPdfAsync(ReportResponseDto reportResult);
}