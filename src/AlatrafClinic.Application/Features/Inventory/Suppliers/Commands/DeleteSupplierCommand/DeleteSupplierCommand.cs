using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.DeleteSupplierCommand;

public sealed record DeleteSupplierCommand(int Id) : IRequest<Result<Deleted>>;