
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.Sales.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<SaleDto>>
{
    private readonly ILogger<CreateSaleCommandHandler> _logger;
    private readonly IAppDbContext _dbContext;
    private readonly IDiagnosisCreationService _diagnosisService;


    public CreateSaleCommandHandler(ILogger<CreateSaleCommandHandler> logger, IAppDbContext dbContext, IDiagnosisCreationService diagnosisService)
    {
        _logger = logger;
        _dbContext = dbContext;
        _diagnosisService = diagnosisService;
    }

    public async Task<Result<SaleDto>> Handle(CreateSaleCommand command, CancellationToken ct)
    {
        if (command.SaleItems is null || command.SaleItems.Count == 0)
            return SaleErrors.SaleItemsAreRequired;

        var diagnosisResult = await _diagnosisService.CreateAsync(
            command.TicketId,
            command.DiagnosisText,
            command.InjuryDate,
            command.InjuryReasons,
            command.InjurySides,
            command.InjuryTypes,
            DiagnosisType.Sales,
            ct);

        if (diagnosisResult.IsError)
        {
            _logger.LogError(
                "Failed to create Diagnosis for Ticket {ticketId}: {Errors}",
                command.TicketId,
                string.Join(", ", diagnosisResult.Errors));

            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;

        var newItems = new List<(ItemUnit itemUnit, decimal quantity)>();

        foreach (var saleItem in command.SaleItems)
        {
            var itemUnit = await _dbContext.ItemUnits
                .SingleOrDefaultAsync(iu => iu.ItemId == saleItem.ItemId && iu.UnitId == saleItem.UnitId, ct);

            if (itemUnit is null)
                return ItemUnitErrors.ItemUnitNotFound;

            if (itemUnit.Price != saleItem.UnitPrice)
                return ItemUnitErrors.InconsistentPrice;

            newItems.Add((itemUnit, saleItem.Quantity));
        }

        var saleResult = Sale.Create(diagnosis.Id, command.Notes);
        if (saleResult.IsError)
            return saleResult.Errors;

        var sale = saleResult.Value;

        var upsertItemsResult = sale.UpsertItems(newItems);
        if (upsertItemsResult.IsError)
            return upsertItemsResult.Errors;

        var assignDiagnosisResult = diagnosis.AssignToSale(sale);
        if (assignDiagnosisResult.IsError)
            return assignDiagnosisResult.Errors;


        var paymentResult = Payment.Create(diagnosis.TicketId, diagnosis.Id, sale.Total, PaymentReference.Sales);

        if (paymentResult.IsError)
        {
            _logger.LogError("Failed to create Payment for Sales : {Errors}", string.Join(", ", paymentResult.Errors));
            return paymentResult.Errors;
        }

        var payment = paymentResult.Value;

        diagnosis.AssignPayment(payment);
        diagnosis.AssignToSale(sale);

        await _dbContext.Diagnoses.AddAsync(diagnosis, ct);
        await _dbContext.SaveChangesAsync(ct);

        // Now Sale.Id is available — raise domain event and persist it
        sale.MarkCreated();
        await _dbContext.SaveChangesAsync(ct);
        _logger.LogInformation(
                   "Created Sale {saleId} for Diagnosis {diagnosisId} and Ticket {ticketId}",
                   sale.Id,
                   diagnosis.Id,
                   command.TicketId);

        return sale.ToDto();
    }
}
