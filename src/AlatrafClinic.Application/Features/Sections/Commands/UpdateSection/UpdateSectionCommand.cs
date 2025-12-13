using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sections.Commands.UpdateSection;

public sealed record UpdateSectionCommand(
    int SectionId,
    int DepartmentId,
    string NewName
) : IRequest<Result<Updated>>;
