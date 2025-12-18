using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Departments.Queries.GetDepartments;

public sealed record GetDepartmentsQuery(
    
) : IRequest<Result<List<DepartmentDto>>>;
// {
//     public string CacheKey => "get-departments";

//     public string[] Tags => ["department"];

//     public TimeSpan Expiration => TimeSpan.FromMinutes(10);
// }
