using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Inventory.Suppliers.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Inventory.Suppliers.Queries.GetAllSuppliersQuery;

public sealed record GetAllSuppliersQuery() : IRequest<Result<List<SupplierDto>>>;
