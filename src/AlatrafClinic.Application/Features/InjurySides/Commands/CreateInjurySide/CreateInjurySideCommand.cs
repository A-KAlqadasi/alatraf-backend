using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurySides.Commands.CreateInjurySide;

public sealed record CreateInjurySideCommand(string Name) : IRequest<Result<InjuryDto>>;