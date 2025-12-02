using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Departments.Queries.GetDepartments;

public sealed record GetDepartmentsQuery(
    
) : ICachedQuery<Result<List<DepartmentDto>>>
{
    public string CacheKey => "department";

    public string[] Tags => ["departments"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
