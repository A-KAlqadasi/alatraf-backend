

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Organization.Departments.Dtos;
using AlatrafClinic.Domain.Common.Results;

namespace AlatrafClinic.Application.Features.Organization.Departments.Queries.GetDepartments;

public sealed record GetDepartmentsQuery(
    string? SearchTerm = null
) : ICachedQuery<Result<List<DepartmentDto>>>
{
    public string CacheKey =>
        $"department:search={SearchTerm?.Trim().ToLower() ?? "all"}";

    public string[] Tags => ["departments"];

    public TimeSpan Expiration => TimeSpan.FromMinutes(10);
}
