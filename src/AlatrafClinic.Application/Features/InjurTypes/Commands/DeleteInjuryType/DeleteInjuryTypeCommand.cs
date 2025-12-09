using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.InjurTypes.Commands.DeleteInjuryType;

public sealed record DeleteInjuryTypeCommand(int InjuryTypeId) : IRequest<Result<Deleted>>;