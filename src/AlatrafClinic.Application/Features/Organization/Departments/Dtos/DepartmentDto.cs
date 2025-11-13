using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AlatrafClinic.Application.Features.Organization.Sections.Dtos;

namespace AlatrafClinic.Application.Features.Organization.Departments.Dtos;
public sealed record DepartmentDto(
    int Id,
    string Name,
    List<SectionDto>? Sections 
);