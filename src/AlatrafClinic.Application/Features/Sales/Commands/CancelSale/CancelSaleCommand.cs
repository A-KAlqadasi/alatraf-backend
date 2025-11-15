using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Commands.CancelSale;

public sealed record CancelSaleCommand(int SaleId) : IRequest<Result<Updated>>;