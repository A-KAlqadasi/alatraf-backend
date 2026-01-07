using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sections.Commands.AssignNewRoomsToSection;

public sealed record AssignNewRoomsToSectionCommand(int SectionId, List<string> RoomNames) : IRequest<Result<Success>>;
