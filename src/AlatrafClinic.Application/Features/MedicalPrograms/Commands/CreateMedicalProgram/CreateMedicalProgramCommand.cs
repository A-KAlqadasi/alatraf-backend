using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Commands.CreateMedicalProgram;

public sealed record CreateMedicalProgramCommand(string Name, string? Description, int? SectionId) : IRequest<Result<MedicalProgramDto>>;