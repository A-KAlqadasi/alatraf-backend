using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.UpdateMedicalProgram;

public sealed record UpdateMedicalProgramCommand(int MedicalProgramId, string Name, string? Description, int? SectionId) : IRequest<Result<Updated>>;