
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Organization.Sections.Commands.UpdateSection;

public sealed record UpdateSectionCommand(
    int SectionId,
    string NewName
) : IRequest<Result<Updated>>;
