using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Commands.CreateSale;

public sealed record class CreateSaleCommand(
    int TicketId,
    string DiagnosisText,
    DateTime InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    int PatientId,
    List<CreateSaleItemCommand> SaleItems,
    string? Notes = null
) : IRequest<Result<SaleDto>>;