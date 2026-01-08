// Application/Sagas/Compensation/SaleSagaCompensationHandler.cs
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Domain.Sagas;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Sagas.Compensation
{
    public class SaleSagaCompensationHandler : ISagaCompensationHandler
    {
        public string SagaType => "SaleSaga";

        private readonly IAppDbContext _db;
        private readonly ILogger<SaleSagaCompensationHandler> _logger;

        public SaleSagaCompensationHandler(IAppDbContext db, ILogger<SaleSagaCompensationHandler> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<CompensationResult> CompensateAsync(Guid sagaId, CancellationToken ct)
        {
            var errors = new List<string>();

            try
            {
                // 1. تحميل حالة الساغا والكيانات المرتبطة
                var sagaState = await _db.SagaStates
                    .Include(s => s.StepRecords)
                    .FirstOrDefaultAsync(s => s.Id == sagaId, ct);

                if (sagaState == null)
                {
                    return  CompensationResult.Failure($"Saga {sagaId} not found");
                }

                sagaState.StartCompensation();
                await _db.SaveChangesAsync(ct);

                // 2. التعويض العكسي للخطوات المنجزة
                var steps = sagaState.StepRecords
                    .Where(sr => sr.Success)
                    .OrderByDescending(sr => sr.ExecutedAt)
                    .ToList();

                foreach (var step in steps)
                {
                    var result = await CompensateStepAsync(step, sagaId, ct);
                    if (!result.Success)
                    {
                        errors.AddRange(result.Errors);
                        _logger.LogWarning("Compensation failed for step {Step} in saga {SagaId}",
                            step.StepName, sagaId);
                    }
                }

                // 3. تحديث حالة الساغا
                if (errors.Count == 0)
                {
                    sagaState.MarkCompensated();
                    await _db.SaveChangesAsync(ct);
                    return  CompensationResult.SuccessResult();
                }
                else
                {
                    return  CompensationResult.Failure(errors.ToArray());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compensate saga {SagaId}", sagaId);
                return  CompensationResult.Failure(ex.Message);
            }
        }

        private async Task<CompensationResult> CompensateStepAsync(
            SagaStepRecord step, Guid sagaId, CancellationToken ct)
        {
            return step.StepName switch
            {
                "CreatePayment" => await CompensatePaymentAsync(sagaId, ct),
                "ConfirmSale" => await CompensateSaleConfirmationAsync(sagaId, ct),
                "ReserveInventory" => await CompensateInventoryReservationAsync(sagaId, ct),
                "CreateDiagnosis" => await CompensateDiagnosisAsync(sagaId, ct),
                "CreateSaleDraft" => await CompensateSaleDraftAsync(sagaId, ct),
                _ =>  CompensationResult.SuccessResult() // لا تعويض للخطوات التي لا تحتاج
            };
        }

        private async Task<CompensationResult> CompensatePaymentAsync(Guid sagaId, CancellationToken ct)
        {
            try
            {
                var payment = await _db.Payments
                    .FirstOrDefaultAsync(p => p.SagaId == sagaId && p.PaymentReference == Domain.Payments.PaymentReference.Sales, ct);

                if (payment != null)
                {
                    // payment.MarkAsCancelled();
                    _logger.LogInformation("Compensated payment {PaymentId} for saga {SagaId}", payment.Id, sagaId);
                }

                await _db.SaveChangesAsync(ct);
                return  CompensationResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compensate payment for saga {SagaId}", sagaId);
                return  CompensationResult.Failure($"Payment compensation failed: {ex.Message}");
            }
        }

        private async Task<CompensationResult> CompensateInventoryReservationAsync(Guid sagaId, CancellationToken ct)
        {
            try
            {
                var reservations = await _db.InventoryReservations
                    .Include(r => r.StoreItemUnit)
                    .Where(r => r.SagaId == sagaId)
                    .ToListAsync(ct);

                foreach (var reservation in reservations)
                {
                    // إعادة الكمية المحجوزة إلى المخزون
                    var storeItemUnit = reservation.StoreItemUnit;
                    storeItemUnit.Increase(reservation.Quantity);

                    // تعليم الحجز كمُعَوَّض
                    reservation.MarkAsCompensated();

                    _logger.LogInformation(
                        "Compensated inventory reservation {ReservationId}: returned {Quantity} to store item {StoreItemUnitId}",
                        reservation.Id, reservation.Quantity, storeItemUnit.Id);
                }

                // تحديث حالة البيع
                var sale = await _db.Sales
                    .FirstOrDefaultAsync(s => s.SagaId == sagaId, ct);

                if (sale != null)
                {
                    sale.MarkInventoryReleased();
                }

                await _db.SaveChangesAsync(ct);
                return  CompensationResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compensate inventory reservations for saga {SagaId}", sagaId);
                return  CompensationResult.Failure($"Inventory compensation failed: {ex.Message}");
            }
        }

        private async Task<CompensationResult> CompensateSaleConfirmationAsync(Guid sagaId, CancellationToken ct)
        {
            try
            {
                var sale = await _db.Sales
                    .FirstOrDefaultAsync(s => s.SagaId == sagaId, ct);

                if (sale != null && sale.Status == Domain.Sales.Enums.SaleStatus.Confirmed)
                {
                    sale.RevertToDraft();
                    _logger.LogInformation("Reverted sale {SaleId} to draft status", sale.Id);
                }

                await _db.SaveChangesAsync(ct);
                return  CompensationResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compensate sale confirmation for saga {SagaId}", sagaId);
                return  CompensationResult.Failure($"Sale confirmation compensation failed: {ex.Message}");
            }
        }

        private async Task<CompensationResult> CompensateDiagnosisAsync(Guid sagaId, CancellationToken ct)
        {
            try
            {
                var diagnosis = await _db.Diagnoses
                    .Include(d => d.Sale)
                    .FirstOrDefaultAsync(d => d.Sale != null && d.Sale.SagaId == sagaId, ct);

                if (diagnosis != null)
                {
                    // إذا لم يكن هناك بيع بعد، يمكن حذف التشخيص
                    if (diagnosis.Sale == null)
                    {
                        _db.Diagnoses.Remove(diagnosis);
                        _logger.LogInformation("Removed diagnosis {DiagnosisId} for saga {SagaId}", diagnosis.Id, sagaId);
                    }
                    else
                    {
                        _logger.LogInformation("Diagnosis {DiagnosisId} has an associated sale, skipping removal", diagnosis.Id);
                    }
                }

                await _db.SaveChangesAsync(ct);
                return  CompensationResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compensate diagnosis for saga {SagaId}", sagaId);
                return  CompensationResult.Failure($"Diagnosis compensation failed: {ex.Message}");
            }
        }

        private async Task<CompensationResult> CompensateSaleDraftAsync(Guid sagaId, CancellationToken ct)
        {
            try
            {
                var sale = await _db.Sales
                    .FirstOrDefaultAsync(s => s.SagaId == sagaId, ct);

                if (sale != null && sale.Status == Domain.Sales.Enums.SaleStatus.Draft)
                {
                    // Soft delete للـ sale draft
                    sale.MarkAsDeleted();
                    _logger.LogInformation("Removed sale draft {SaleId} for saga {SagaId}", sale.Id, sagaId);
                }

                await _db.SaveChangesAsync(ct);
                return  CompensationResult.SuccessResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compensate sale draft for saga {SagaId}", sagaId);
                return  CompensationResult.Failure($"Sale draft compensation failed: {ex.Message}");
            }
        }
    }
}