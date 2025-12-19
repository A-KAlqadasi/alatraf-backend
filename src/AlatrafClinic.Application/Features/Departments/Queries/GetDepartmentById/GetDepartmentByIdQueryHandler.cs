using AlatrafClinic.Application.Common.Errors;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features;
using AlatrafClinic.Application.Features.Departments.Dtos;
using AlatrafClinic.Application.Features.Departments.Mappers;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace AlatrafClinic.Application.Features.Departments.Queries.GetDepartmentById;

public sealed class GetDepartmentByIdQueryHandler(
    IAppDbContext _context
) : IRequestHandler<GetDepartmentByIdQuery, Result<DepartmentDto>>
{

    public async Task<Result<DepartmentDto>> Handle(GetDepartmentByIdQuery query, CancellationToken ct)
    {
        var department = await _context.Departments.Include(d=> d.Sections).FirstOrDefaultAsync(d=> d.Id == query.DepartmentId, ct);

        if (department is null)
        return ApplicationErrors.DepartmentNotFound;

        return department.ToDto();
    }
}