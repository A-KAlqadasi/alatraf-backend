using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Commands.CreateSale;

public sealed record class CreateSaleCommand(
    int TicketId,
    string DiagnosisText,
    DateOnly InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    List<CreateSaleItemCommand> SaleItems,
    string? Notes = null
) : IRequest<Result<SaleDto>>;