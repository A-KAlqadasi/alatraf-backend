using AlatrafClinic.Domain.Reports;

namespace AlatrafClinic.Application.Reports.Interfaces;

public interface IReportMetadataRepository
{
    Task<ReportDomainWithRelations> GetDomainWithRelationsAsync(int domainId);
}