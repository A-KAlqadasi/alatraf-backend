using System;
using MediatR;
using AlatrafClinic.Application.Sagas;

namespace AlatrafClinic.Application.Commands;

public sealed record ConfirmSaleCommand(
    Guid SagaId,
    int SaleId
) : IRequest<SaleSagaResult>;
