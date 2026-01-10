// Application/Sagas/SaleSagaOrchestrator.cs (معدل)
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using AlatrafClinic.Application.Commands;
using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Application.Sagas.Compensation;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Inventory.Reservations;
using AlatrafClinic.Domain.Inventory.Stores;
using AlatrafClinic.Domain.Sagas;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Sales.Enums;
using AlatrafClinic.Domain.Sales.SalesItems;

namespace AlatrafClinic.Application.Sagas
{
    public sealed class SaleSagaOrchestrator
    {
        private readonly IAppDbContext _db;
        private readonly ILogger<SaleSagaOrchestrator> _logger;
        private readonly IDiagnosisCreationService _diagnosisService;
        private readonly ISagaStateService _sagaStateService;
        private readonly ISagaCompensationHandler _compensationHandler;

        public SaleSagaOrchestrator(
            IAppDbContext db,
            ILogger<SaleSagaOrchestrator> logger,
            IDiagnosisCreationService diagnosisService,
            ISagaStateService sagaStateService,
            ISagaCompensationHandler compensationHandler)
        {
            _db = db;
            _logger = logger;
            _diagnosisService = diagnosisService;
            _sagaStateService = sagaStateService;
            _compensationHandler = compensationHandler;
        }

        /// <summary>
        /// تنفيذ جميع مراحل البيع في طلب واحد مع Rollback تلقائي عند الفشل
        /// </summary>
        public async Task<SaleSagaResult> ProcessCompleteSaleAsync(StartSaleSagaCommand command, CancellationToken ct)
        {
            var sagaId = command.SagaId == Guid.Empty ? Guid.NewGuid() : command.SagaId;

            // التحقق من أن الـ DbContext يدعم Transaction
            if (_db is not DbContext efDb)
            {
                return SaleSagaResult.Fail("DbContext لا يدعم المعاملات (Transactions).");
            }

            // إنشاء/تحديث حالة الساغا
            await _sagaStateService.StartOrContinueSagaAsync(
                sagaId, "CompleteSaleSaga", "StartingTransaction", ct);

            // بدء Transaction واحدة لجميع العمليات
            await using var transaction = await efDb.Database.BeginTransactionAsync(ct);

            try
            {
                _logger.LogInformation("بدء معاملة البيع الكاملة للساغا {SagaId}", sagaId);
                await _sagaStateService.RecordStepAsync(sagaId, "StartingTransaction", ct);

                // الخطوة 1: التحقق من المخزون
                await _sagaStateService.RecordStepAsync(sagaId, "ValidateStock", ct);
                var stockOk = await ValidateStockAsync(command.Items, ct);
                if (!stockOk)
                {
                    await _sagaStateService.RecordStepFailureAsync(
                        sagaId, "ValidateStock", "المخزون غير كافٍ", ct);
                    await transaction.RollbackAsync(ct);
                    return SaleSagaResult.Fail("المخزون غير كافٍ لواحد أو أكثر من العناصر.");
                }
                await _sagaStateService.RecordStepSuccessAsync(sagaId, "ValidateStock", ct);

                // الخطوة 2: إنشاء التشخيص
                await _sagaStateService.RecordStepAsync(sagaId, "CreateDiagnosis", ct);
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
                    await _sagaStateService.RecordStepFailureAsync(
                        sagaId, "CreateDiagnosis",
                        string.Join(", ", diagnosisResult.Errors.Select(e => e.Description)), ct);
                    await transaction.RollbackAsync(ct);
                    return SaleSagaResult.Fail(diagnosisResult.Errors.Select(e => e.Description).ToArray());
                }

                var diagnosis = diagnosisResult.Value;
                _db.Diagnoses.Attach(diagnosis);
                await _db.SaveChangesAsync(ct);
                await _sagaStateService.RecordStepSuccessAsync(sagaId, "CreateDiagnosis", ct);

                // الخطوة 3: إنشاء مسودة البيع
                await _sagaStateService.RecordStepAsync(sagaId, "CreateSaleDraft", ct);
                var draftResult = await CreateSaleDraftInternalAsync(sagaId, command, diagnosis.Id, ct);
                if (!draftResult.IsSuccess)
                {
                    await _sagaStateService.RecordStepFailureAsync(
                        sagaId, "CreateSaleDraft",
                        string.Join(", ", draftResult.Errors), ct);
                    await transaction.RollbackAsync(ct);
                    return draftResult.Errors.Length == 1
                        ? SaleSagaResult.Fail(draftResult.Errors[0])
                        : SaleSagaResult.Fail(draftResult.Errors);
                }
                await _sagaStateService.RecordStepSuccessAsync(sagaId, "CreateSaleDraft", ct);

                var sale = draftResult.Value; // Sale entity

                // الخطوة 4: حجز المخزون
                await _sagaStateService.RecordStepAsync(sagaId, "ReserveInventory", ct);
                var reserveResult = await ReserveInventoryInternalAsync(sagaId, sale, ct);
                if (!reserveResult.IsSuccess)
                {
                    await _sagaStateService.RecordStepFailureAsync(
                        sagaId, "ReserveInventory",
                        string.Join(", ", reserveResult.Errors), ct);
                    await transaction.RollbackAsync(ct);
                    return reserveResult;
                }
                await _sagaStateService.RecordStepSuccessAsync(sagaId, "ReserveInventory", ct);

                // الخطوة 5: تأكيد البيع
                await _sagaStateService.RecordStepAsync(sagaId, "ConfirmSale", ct);
                var confirmResult = await ConfirmSaleInternalAsync(sagaId, sale, ct);
                if (!confirmResult.IsSuccess)
                {
                    await _sagaStateService.RecordStepFailureAsync(
                        sagaId, "ConfirmSale",
                        string.Join(", ", confirmResult.Errors), ct);
                    await transaction.RollbackAsync(ct);
                    return confirmResult;
                }
                await _sagaStateService.RecordStepSuccessAsync(sagaId, "ConfirmSale", ct);

                // الخطوة 6: إنشاء الدفع
                await _sagaStateService.RecordStepAsync(sagaId, "CreatePayment", ct);
                var paymentResult = await CreatePaymentInternalAsync(sagaId, sale, ct);
                if (!paymentResult.IsSuccess)
                {
                    await _sagaStateService.RecordStepFailureAsync(
                        sagaId, "CreatePayment",
                        string.Join(", ", paymentResult.Errors), ct);
                    await transaction.RollbackAsync(ct);
                    return paymentResult;
                }
                await _sagaStateService.RecordStepSuccessAsync(sagaId, "CreatePayment", ct);

                // حفظ جميع التغييرات وإكمال المعاملة
                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                _logger.LogInformation("تم إكمال معاملة البيع بنجاح للساغا {SagaId}، رقم البيع: {SaleId}", sagaId, sale.Id);

                // تحديث حالة الساغا كمنتهية بنجاح
                await _sagaStateService.MarkSagaCompletedAsync(sagaId, "CompletedSuccessfully", ct);

                return SaleSagaResult.Ok(sale.Id, sale.Total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل الساغا {SagaId} أثناء المعاملة", sagaId);

                try
                {
                    // محاولة Rollback للمعاملة
                    await transaction.RollbackAsync(ct);
                    _logger.LogInformation("تم التراجع عن المعاملة للساغا {SagaId}", sagaId);
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, "فشل في التراجع عن المعاملة للساغا {SagaId}", sagaId);
                }

                await _sagaStateService.RecordStepFailureAsync(
                    sagaId, "TransactionFailed", ex.Message, ct);

                // محاولة التعويض التلقائي لأي عمليات خارجية
                await TryAutoCompensateAsync(sagaId, ex, ct);

                return SaleSagaResult.Fail($"فشل العملية: {ex.Message}");
            }
        }

        #region الدوال المساعدة الداخلية

        private async Task<SaleSagaResultInternal<Sale>> CreateSaleDraftInternalAsync(
            Guid sagaId,
            StartSaleSagaCommand command,
            int diagnosisId,
            CancellationToken ct)
        {
            try
            {
                var newItems = new List<(Domain.Inventory.Items.ItemUnit itemUnit, decimal quantity)>();
                foreach (var item in command.Items)
                {
                    var itemUnit = await _db.ItemUnits
                        .FirstOrDefaultAsync(iu => iu.ItemId == item.ItemId && iu.Id == item.UnitId, ct);

                    if (itemUnit is null)
                    {
                        return SaleSagaResultInternal<Sale>.Fail($"الوحدة {item.UnitId} غير موجودة.");
                    }

                    if (itemUnit.Price != item.UnitPrice)
                    {
                        return SaleSagaResultInternal<Sale>.Fail($"سعر الوحدة {item.UnitId} غير متطابق.");
                    }

                    newItems.Add((itemUnit, item.Quantity));
                }

                var saleResult = Sale.Create(diagnosisId, command.Notes);
                if (saleResult.IsError)
                {
                    return SaleSagaResultInternal<Sale>.Fail(saleResult.Errors.Select(e => e.Description).ToArray());
                }

                var sale = saleResult.Value;

                // ربط الساغا بالبيع
                var sagaAttach = sale.AttachSaga(sagaId);
                if (sagaAttach.IsError)
                {
                    return SaleSagaResultInternal<Sale>.Fail(sagaAttach.Errors.Select(e => e.Description).ToArray());
                }

                // إضافة العناصر
                var upsertResult = sale.UpsertItems(newItems);
                if (upsertResult.IsError)
                {
                    return SaleSagaResultInternal<Sale>.Fail(upsertResult.Errors.Select(e => e.Description).ToArray());
                }

                await _db.Sales.AddAsync(sale, ct);
                return SaleSagaResultInternal<Sale>.Success(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل في إنشاء مسودة البيع للساغا {SagaId}", sagaId);
                return SaleSagaResultInternal<Sale>.Fail($"فشل في إنشاء مسودة البيع: {ex.Message}");
            }
        }

        private async Task<SaleSagaResult> ReserveInventoryInternalAsync(Guid sagaId, Sale sale, CancellationToken ct)
        {
            try
            {
                // التحقق من أن البيع يحتوي على عناصر
                if (!sale.SaleItems.Any())
                {
                    return SaleSagaResult.Fail("لا توجد عناصر في البيع.");
                }

                // تحميل عناصر البيع مع الوحدات
                // بدلاً من استخدام Entry، نقوم بتحميل SaleItems بشكل مباشر
                var saleItemUnits = sale.SaleItems.Select(si => si.ItemUnitId).ToList();
                var storeUnits = await _db.StoreItemUnits
                    .Where(x => saleItemUnits.Contains(x.ItemUnitId))
                    .ToListAsync(ct);

                // التحقق من توفر المخزون لجميع العناصر
                foreach (var saleItem in sale.SaleItems)
                {
                    var stock = storeUnits.FirstOrDefault(su => su.ItemUnitId == saleItem.ItemUnitId);
                    if (stock is null || stock.Quantity < saleItem.Quantity)
                    {
                        return SaleSagaResult.Fail($"المخزون غير كافٍ للوحدة {saleItem.ItemUnitId}");
                    }
                }

                // حجز المخزون وإنشاء سجلات الحجز
                foreach (var saleItem in sale.SaleItems)
                {
                    var storeItemUnit = storeUnits.First(su => su.ItemUnitId == saleItem.ItemUnitId);

                    var decrease = storeItemUnit.Decrease(saleItem.Quantity);
                    if (decrease.IsError)
                    {
                        return SaleSagaResult.Fail($"فشل في تقليل المخزون للوحدة {saleItem.ItemUnitId}");
                    }

                    saleItem.AssignStoreItemUnit(storeItemUnit);
                    var reservation = InventoryReservation.Create(sagaId, sale.Id, storeItemUnit.Id, saleItem.Quantity);
                    await _db.InventoryReservations.AddAsync(reservation, ct);
                }

                // تحديث حالة البيع
                var markReserved = sale.MarkInventoryReserved(sagaId);
                if (markReserved.IsError)
                {
                    return SaleSagaResult.Fail(markReserved.Errors.Select(e => e.Description).ToArray());
                }

                return SaleSagaResult.Ok(sale.Id, sale.Total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل في حجز المخزون للساغا {SagaId}", sagaId);
                return SaleSagaResult.Fail($"فشل في حجز المخزون: {ex.Message}");
            }
        }

        private async Task<SaleSagaResult> ConfirmSaleInternalAsync(Guid sagaId, Sale sale, CancellationToken ct)
        {
            try
            {
                // التحقق من حجز المخزون
                if (!sale.InventoryReservationCompleted)
                {
                    return SaleSagaResult.Fail("لم يتم حجز المخزون بعد؛ لا يمكن تأكيد البيع.");
                }

                // إذا كان البيع مؤكداً بالفعل
                if (sale.Status == SaleStatus.Confirmed)
                {
                    return SaleSagaResult.Success();
                }

                // تأكيد البيع
                var result = sale.Confirm(sagaId);
                if (result.IsError)
                {
                    return SaleSagaResult.Fail(result.Errors.Select(e => e.Description).ToArray());
                }

                return SaleSagaResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل في تأكيد البيع للساغا {SagaId}", sagaId);
                return SaleSagaResult.Fail($"فشل في تأكيد البيع: {ex.Message}");
            }
        }

        private async Task<SaleSagaResult> CreatePaymentInternalAsync(Guid sagaId, Sale sale, CancellationToken ct)
        {
            try
            {
                // تحميل التشخيص والمدفوعات
                var diagnosis = await _db.Diagnoses.Include(s => s.Payments).FirstOrDefaultAsync(d => d.Id == sale.DiagnosisId, ct);

                if (diagnosis is null)
                {
                    return SaleSagaResult.Fail($"التشخيص غير موجود للبيع {sale.Id}.");
                }

                // التحقق من الشروط المسبقة
                if (!sale.InventoryReservationCompleted)
                {
                    return SaleSagaResult.Fail("لم يتم حجز المخزون بعد؛ لا يمكن إنشاء الدفع.");
                }

                if (sale.Status != SaleStatus.Confirmed)
                {
                    return SaleSagaResult.Fail("يجب تأكيد البيع قبل إنشاء الدفع.");
                }


                // التحقق من عدم وجود دفع مسبق لهذه الساغا
                var existing = sale.Diagnosis.Payments
                    .FirstOrDefault(p => p.SagaId == sagaId && p.PaymentReference == Domain.Payments.PaymentReference.Sales);

                if (existing is not null)
                {
                    return SaleSagaResult.Success();
                }

                // إنشاء الدفع
                var paymentResult = Domain.Payments.Payment.Create(
                    sagaId,
                    sale.Diagnosis.TicketId,
                    sale.DiagnosisId,
                    sale.Total,
                    Domain.Payments.PaymentReference.Sales);

                if (paymentResult.IsError)
                {
                    return SaleSagaResult.Fail(paymentResult.Errors.Select(e => e.Description).ToArray());
                }

                var payment = paymentResult.Value;
                sale.Diagnosis.AssignPayment(payment);

                await _db.Payments.AddAsync(payment, ct);

                // تحديث حالة البيع
                var markPayment = sale.MarkPaymentCreated(sagaId);
                if (markPayment.IsError)
                {
                    return SaleSagaResult.Fail(markPayment.Errors.Select(e => e.Description).ToArray());
                }

                return SaleSagaResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "فشل في إنشاء الدفع للساغا {SagaId}", sagaId);
                return SaleSagaResult.Fail($"فشل في إنشاء الدفع: {ex.Message}");
            }
        }

        private async Task<bool> ValidateStockAsync(IReadOnlyCollection<SaleItemInput> items, CancellationToken ct)
        {
            if (!items.Any()) return false;

            var itemUnitIds = items.Select(i => i.UnitId).ToList();
            var stocks = await _db.StoreItemUnits
                .Where(siu => itemUnitIds.Contains(siu.ItemUnitId))
                .ToListAsync(ct);

            foreach (var item in items)
            {
                var stock = stocks.FirstOrDefault(s => s.ItemUnitId == item.UnitId);
                if (stock is null) return false;
                if (stock.Quantity < item.Quantity) return false;
            }

            return true;
        }

        private async Task TryAutoCompensateAsync(Guid sagaId, Exception exception, CancellationToken ct)
        {
            try
            {
                _logger.LogWarning("محاولة التعويض التلقائي للساغا {SagaId}", sagaId);
                var result = await _compensationHandler.CompensateAsync(sagaId, ct);

                if (result.Success)
                {
                    _logger.LogInformation("تم التعويض بنجاح للساغا {SagaId}", sagaId);
                }
                else
                {
                    _logger.LogError("فشل التعويض التلقائي للساغا {SagaId}: {Errors}",
                        sagaId, string.Join(", ", result.Errors));
                }
            }
            catch (Exception compEx)
            {
                _logger.LogError(compEx, "فشل في تنفيذ التعويض التلقائي للساغا {SagaId}", sagaId);
            }
        }

        #endregion

        #region فئات النتائج المساعدة (الداخلية)

        internal class SaleSagaResultInternal<T> where T : class
        {
            public bool IsSuccess { get; }
            public T? Value { get; }
            public string[] Errors { get; }

            private SaleSagaResultInternal(bool isSuccess, T? value, string[] errors)
            {
                IsSuccess = isSuccess;
                Value = value;
                Errors = errors ?? Array.Empty<string>();
            }

            public static SaleSagaResultInternal<T> Success(T value)
                => new(true, value, Array.Empty<string>());

            public static SaleSagaResultInternal<T> Fail(params string[] errors)
                => new(false, default, errors);
        }

        #endregion
    }
}