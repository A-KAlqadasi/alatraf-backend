using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Hybrid;

using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Sales.Enums;
using AlatrafClinic.Domain.Payments;

using MediatR;


namespace AlatrafClinic.Application.Features.Sales.Commands.UpdateSale;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, Result<Updated>>
{
    private readonly ILogger<UpdateSaleCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HybridCache _cache;
    private readonly IDiagnosisUpdateService _diagnosisUpdate;

    public UpdateSaleCommandHandler(ILogger<UpdateSaleCommandHandler> logger, IUnitOfWork unitOfWork, HybridCache cache, IDiagnosisUpdateService diagnosisUpdate)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cache = cache;
        _diagnosisUpdate = diagnosisUpdate;
    }
    public async Task<Result<Updated>> Handle(UpdateSaleCommand command, CancellationToken ct)
    {
        Sale? currentSale = await _unitOfWork.Sales.GetByIdAsync(command.SaleId, ct);
        if (currentSale is null)
        {
            _logger.LogError("Sale with id {SaleId} not found", command.SaleId);

            return SaleErrors.SaleNotFound;
        }

        if (currentSale.Status != SaleStatus.Draft)
        {
            _logger.LogError("Sale {saleId} cannot be modified, read-only", currentSale.Id);
            return SaleErrors.Readonly;
        }

        if (currentSale.IsPaid)
        {
            _logger.LogError("Sale {saleId} cannot be modified, it is paid", currentSale.Id);
            return SaleErrors.Readonly;
        }

        var updateDiagnosisResult = await _diagnosisUpdate.UpdateAsync(
            diagnosisId: currentSale.DiagnosisId,
            ticketId: command.TicketId,
            diagnosisText: command.DiagnosisText,
            injuryDate: command.InjuryDate,
            injuryReasons: command.InjuryReasons,
            injurySides: command.InjurySides,
            injuryTypes: command.InjuryTypes,
            DiagnosisType.Sales,
            ct: ct);

        if (updateDiagnosisResult.IsError)
        {
            _logger.LogError("Failed to update diagnosis {diagnosisId} for Sale with id {saleId}", currentSale.DiagnosisId, command.SaleId);
            return updateDiagnosisResult.Errors;
        }

        var diagnosis = updateDiagnosisResult.Value;
        
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

        currentSale.Notes = command.Notes;

        var upsertItemsResult = currentSale.UpsertItems(newItems);
        if (upsertItemsResult.IsError)
        {
            _logger.LogError("Failed to update items to Sale {saleId}: {Errors}", command.SaleId, string.Join(", ", upsertItemsResult.Errors));
            return upsertItemsResult.Errors;
        }

        var assignDiagnosisResult = diagnosis.AssignToSale(currentSale);
        if (assignDiagnosisResult.IsError)
        {
            _logger.LogError("Failed to assign Diagnosis {diagnosisId} to Sale {saleId}: {Errors}", diagnosis.Id, command.SaleId, string.Join(", ", assignDiagnosisResult.Errors));
            return assignDiagnosisResult.Errors;
        }

        var currentPayment = currentSale.Payment;
        if (currentPayment is null)
        {
            _logger.LogError("Payment for Sale with id {SaleId} not found", command.SaleId);
            return SaleErrors.PaymentNotFound;
        }

        var updatePaymentResult = currentPayment.UpdateCore(
            ticketId: diagnosis.TicketId,
            diagnosisId: diagnosis.Id,
            total: currentSale.Total,
            reference: PaymentReference.Sales);
        
        if (updatePaymentResult.IsError)
        {
            _logger.LogError("Failed to update payment for Sale with id {saleId}: {Errors}", command.SaleId, string.Join(", ", updatePaymentResult.Errors));
            return updatePaymentResult.Errors;
        }

        diagnosis.AssignPayment(currentPayment);

        await _unitOfWork.Diagnoses.UpdateAsync(diagnosis, ct);
        await _unitOfWork.Sales.UpdateAsync(currentSale, ct);
        await _unitOfWork.Payments.UpdateAsync(currentPayment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        _logger.LogInformation("Sale {saleId} updated successfully", currentSale.Id);

        return Result.Updated;
    }
}