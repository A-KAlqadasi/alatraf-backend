using AlatrafClinic.Application.Common.Models;
using AlatrafClinic.Application.Reports.CountableReports.Dtos;

using MediatR;

namespace AlatrafClinic.Application.Reports.CountableReports.ReportBasedOnInjury;

public sealed record ReportBasedOnInjuryQuery(
    int PageNumber,
    int PageSize,
    DateOnly? From = null ,
    DateOnly? To = null,
    
    ReportBasedOnInjury? BasedOn = null

) : IRequest<PaginatedList<ReportBasedOnInjuryDto>>;

public enum ReportBasedOnInjury
{
    InjuryType,
    InjuryReason,
    InjurySide 
}
