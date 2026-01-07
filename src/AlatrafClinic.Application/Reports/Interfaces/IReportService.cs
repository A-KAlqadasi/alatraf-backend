using AlatrafClinic.Application.Reports.Dtos;

namespace AlatrafClinic.Application.Reports.Interfaces;

public interface IReportService
{
    Task<ReportResponseDto> RunAsync(ReportRequestDto request);
    
    // Add these methods for the controller
    Task<IEnumerable<ReportDomainDto>> GetAvailableDomainsAsync();
    Task<IEnumerable<ReportFieldDto>> GetDomainFieldsAsync(int domainId);
}