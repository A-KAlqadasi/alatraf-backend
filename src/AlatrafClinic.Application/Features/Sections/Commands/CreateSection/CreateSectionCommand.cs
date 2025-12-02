using AlatrafClinic.Application.Features.Sections.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sections.Commands.CreateSection;

public sealed record CreateSectionCommand(
    int DepartmentId,
    string Name
) : IRequest<Result<SectionDto>>;
