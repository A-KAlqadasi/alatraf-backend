using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetSupplierByIdQuery;

public sealed record GetSupplierByIdQuery(int Id) : IRequest<Result<SupplierDto>>; 
