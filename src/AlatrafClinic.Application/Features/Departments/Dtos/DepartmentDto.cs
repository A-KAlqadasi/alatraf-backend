using AlatrafClinic.Application.Features.Sections.Dtos;

namespace AlatrafClinic.Application.Features.Departments.Dtos;
public sealed record DepartmentDto(
    int Id,
    string Name,
    List<SectionDto>? Sections 
);