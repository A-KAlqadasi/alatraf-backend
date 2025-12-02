using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sections.Commands.DeleteSection;

public sealed record class DeleteSectionCommand(
    int SectionId
) : IRequest<Result<Deleted>>;