using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Application.Features.Departments.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Departments.Queries.GetDepartments;

public sealed class GetDepartmentsQueryHandler(
    IAppDbContext _context
) : IRequestHandler<GetDepartmentsQuery, Result<List<DepartmentDto>>>
{

  public async Task<Result<List<DepartmentDto>>> Handle(GetDepartmentsQuery request, CancellationToken ct)
  {
     var departments = await _context.Departments.Include(d=> d.Sections).ToListAsync();

    return departments.ToDtos();
  }
}