using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.DeleteMedicalProgram;

public sealed record DeleteMedicalProgramCommand(int MedicalProgramId) : IRequest<Result<Deleted>>;