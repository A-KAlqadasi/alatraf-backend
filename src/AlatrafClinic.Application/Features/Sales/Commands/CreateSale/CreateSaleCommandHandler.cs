
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Application.Features.Sales.Dtos;
using AlatrafClinic.Application.Features.Sales.Mappers;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Payments;
using AlatrafClinic.Domain.Sales;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<SaleDto>>
{
    private readonly ILogger<CreateSaleCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiagnosisCreationService _diagnosisService;
    private readonly HybridCache _cache;

    public CreateSaleCommandHandler(ILogger<CreateSaleCommandHandler> logger, IUnitOfWork unitOfWork, IDiagnosisCreationService diagnosisService, HybridCache cache)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _diagnosisService = diagnosisService;
        _cache = cache;
    }

    public async Task<Result<SaleDto>> Handle(CreateSaleCommand command, CancellationToken ct)
    {
        if (command.SaleItems is null || command.SaleItems.Count == 0)
        {
            return SaleErrors.SaleItemsAreRequired;
        }

        var diagnosisResult = await _diagnosisService.CreateAsync(
            command.TicketId,
            command.DiagnosisText,
            command.InjuryDate,
            command.InjuryReasons,
            command.InjurySides,
            command.InjuryTypes,
            command.PatientId,
            DiagnosisType.Sales,
            ct);

        if (diagnosisResult.IsError)
        {
            _logger.LogError("Failed to create Diagnosis for Ticket {ticketId}: {Errors}", command.TicketId, string.Join(", ", diagnosisResult.Errors));
            return diagnosisResult.Errors;
        }

        var diagnosis = diagnosisResult.Value;
        List<(ItemUnit itemUnit, decimal quantity)> newItems = new();

        foreach (var saleItem in command.SaleItems)
        {
            var itemUnit = await _unitOfWork.Items.GetByIdAndUnitIdAsync(saleItem.ItemId, saleItem.UnitId, ct);
            if (itemUnit is null)
            {
                _logger.LogError("Item {itemId} dosn't have unit {unitId}.", saleItem.ItemId, saleItem.UnitId);
                return ItemUnitErrors.ItemUnitNotFound;
            }

            if (itemUnit.Price != saleItem.UnitPrice)
            {
                _logger.LogError("Price for unit is not consistant incoming {incomingPrice} and storedPrice {storedPrice}", saleItem.UnitPrice, itemUnit.Price);
                return ItemUnitErrors.InconsistentPrice;
            }

            newItems.Add((itemUnit, saleItem.Quantity));
        }
        var saleResult = Sale.Create(diagnosis.Id, command.Notes);
        if (saleResult.IsError)
        {
            _logger.LogError("Failed to create Sale for Ticket {ticketId}: {Errors}", command.TicketId, string.Join(", ", saleResult.Errors));
            return saleResult.Errors;
        }

        var sale = saleResult.Value;
        
        var upsertItemsResult = sale.UpsertItems(newItems);
        if (upsertItemsResult.IsError)
        {
            _logger.LogError("Failed to add items to Sale for Ticket {ticketId}: {Errors}", command.TicketId, string.Join(", ", upsertItemsResult.Errors));
            return upsertItemsResult.Errors;
        }

        var assignDiagnosisResult = diagnosis.AssignToSale(sale);
        if (assignDiagnosisResult.IsError)
        {
            _logger.LogError("Failed to assign Diagnosis to Sale for Ticket {ticketId}: {Errors}", command.TicketId, string.Join(", ", assignDiagnosisResult.Errors));
            return assignDiagnosisResult.Errors;
        }

        var paymentResult = Payment.Create(diagnosis.TicketId, diagnosis.Id, sale.Total, PaymentReference.Sales);

        if (paymentResult.IsError)
        {
            _logger.LogError("Failed to create Payment for Sales : {Errors}", string.Join(", ", paymentResult.Errors));
            return paymentResult.Errors;
        }

        var payment = paymentResult.Value;

        diagnosis.AssignPayment(payment);

        await _unitOfWork.Diagnoses.AddAsync(diagnosis, ct);
        await _unitOfWork.Sales.AddAsync(sale, ct);
        await _unitOfWork.Payments.AddAsync(payment, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        _logger.LogInformation("Created Sale {saleId} for Diagnosis {diagnosisId} and ticket {ticketId}.", sale.Id, diagnosis.Id, command.TicketId);
        
        return sale.ToDto();
    }
}