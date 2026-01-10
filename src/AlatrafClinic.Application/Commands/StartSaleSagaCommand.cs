using System;
using System.Collections.Generic;
using MediatR;
using AlatrafClinic.Application.Sagas;

namespace AlatrafClinic.Application.Commands;

public sealed record StartSaleSagaCommand(
    Guid SagaId,
    int TicketId,
    string DiagnosisText,
    DateOnly InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    List<SaleItemInput> Items,
    string? Notes
) : IRequest<SaleSagaResult>;

public sealed record SaleItemInput(int ItemId, int UnitId, decimal Quantity, decimal UnitPrice);
