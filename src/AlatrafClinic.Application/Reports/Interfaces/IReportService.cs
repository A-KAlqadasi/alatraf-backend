using AlatrafClinic.Application.Reports.Dtos;

namespace AlatrafClinic.Application.Reports.Interfaces;

public interface IReportService
{
    Task<ReportResponseDto> RunAsync(ReportRequestDto request);
}
