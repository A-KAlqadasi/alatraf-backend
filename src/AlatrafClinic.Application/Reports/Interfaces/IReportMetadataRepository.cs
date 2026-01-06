using AlatrafClinic.Domain.Reports;

namespace AlatrafClinic.Application.Reports.Interfaces;

public interface IReportMetadataRepository
{
    Task<ReportDomain> GetDomainAsync(int domainId);
    Task<List<ReportField>> GetFieldsAsync(int domainId);
    Task<List<ReportJoin>> GetJoinsAsync(int domainId);
}