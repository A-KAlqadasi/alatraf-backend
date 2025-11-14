using AlatrafClinic.Domain.Common.Results;
using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Items.Commands.DeleteItemCommand;

public record DeleteItemCommand(int Id) : IRequest<Result<Deleted>>;
