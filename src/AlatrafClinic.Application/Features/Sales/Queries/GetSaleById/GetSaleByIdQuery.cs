using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Queries.GetSaleById;

public sealed record GetSaleByIdQuery(int SaleId) : IRequest<Result<SaleDto>>;