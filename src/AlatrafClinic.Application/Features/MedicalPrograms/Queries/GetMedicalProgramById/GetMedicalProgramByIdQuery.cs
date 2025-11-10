using AlatrafClinic.Application.Features.MedicalPrograms.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.MedicalPrograms.Queries.GetMedicalProgramById;

public sealed record GetMedicalProgramByIdQuery(int MedicalProgramId) : IRequest<Result<MedicalProgramDto>>;