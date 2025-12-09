using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.CreateInjuryType;

public sealed record CreateInjuryTypeCommand(string Name) : IRequest<Result<InjuryDto>>;