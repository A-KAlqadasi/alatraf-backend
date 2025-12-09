using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.UpdateInjuryType;

public sealed record UpdateInjuryTypeCommand(int InjuryTypeId, string Name) : IRequest<Result<Updated>>;