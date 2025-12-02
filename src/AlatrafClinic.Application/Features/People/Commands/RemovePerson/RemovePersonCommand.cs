using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.People.Commands.RemovePerson;

public sealed record RemovePersonCommand(int PersonId)
: IRequest<Result<Deleted>>;
