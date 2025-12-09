using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjuryReasons.Commands.CreateInjuryReason;

public sealed record CreateInjuryReasonCommand(string Name) : IRequest<Result<InjuryDto>>;