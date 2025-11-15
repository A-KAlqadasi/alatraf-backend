using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Domain.Common.Results;

using MediatR;

namespace AlatrafClinic.Application.Features.Sales.Commands.UpdateSale;

public sealed record class UpdateSaleCommand(
    int SaleId,
    int TicketId,
    string DiagnosisText,
    DateTime InjuryDate,
    List<int> InjuryReasons,
    List<int> InjurySides,
    List<int> InjuryTypes,
    int PatientId,
    List<UpdateSaleItemCommand> SaleItems,
    string? Notes = null
) : IRequest<Result<Updated>>;