using System;
using System.Collections.Generic;
using MediatR;
using AlatrafClinic.Application.Sagas;

namespace AlatrafClinic.Application.Commands;

public sealed record ReserveInventoryCommand(
    Guid SagaId,
    int SaleId,
    IReadOnlyCollection<SaleItemReservationRequest> Items
) : IRequest<SaleSagaResult>;

public sealed record SaleItemReservationRequest(int ItemUnitId, decimal Quantity);
