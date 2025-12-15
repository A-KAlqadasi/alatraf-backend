using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Commands.DeleteSale;

public sealed record DeleteSaleCommand(int SaleId) : IRequest<Result<Updated>>;