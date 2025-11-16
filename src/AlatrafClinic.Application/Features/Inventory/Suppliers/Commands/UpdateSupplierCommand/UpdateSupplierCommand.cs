using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Commands.UpdateSupplierCommand;

public sealed record UpdateSupplierCommand(
    int Id,
    string SupplierName,
    string Phone
) : IRequest<Result<SupplierDto>>;