using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.DisabledCards.Commands.DeleteDisabledCard;

public sealed record class DeleteDisabledCardCommand(int DisabledCardId) : IRequest<Result<Deleted>>;
