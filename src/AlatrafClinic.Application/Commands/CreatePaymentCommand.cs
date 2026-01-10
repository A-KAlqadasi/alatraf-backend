using System;
using MediatR;
using AlatrafClinic.Application.Sagas;

namespace AlatrafClinic.Application.Commands;

public sealed record CreatePaymentCommand(
    Guid SagaId,
    int SaleId,
    decimal Total
) : IRequest<SaleSagaResult>;
