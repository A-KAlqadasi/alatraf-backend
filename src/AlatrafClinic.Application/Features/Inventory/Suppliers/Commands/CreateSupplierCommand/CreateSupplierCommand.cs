using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;
namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.CreateSupplierCommand;

public sealed record CreateSupplierCommand(
    string SupplierName,
    string Phone
) : IRequest<Result<SupplierDto>>;