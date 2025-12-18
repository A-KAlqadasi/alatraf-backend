
using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sections.Queries.GetDepartmentSections;

public sealed record GetDepartmentSectionsQuery(int DepartmentId) : IRequest<Result<List<DepartmentSectionDto>>>;
// {
//     public string CacheKey => $"GetDepartmentSectionsQuery_DepartmentId_{DepartmentId}";
//     public string[] Tags => new[] { "section" };
//     public TimeSpan Expiration => TimeSpan.FromHours(1);
// }