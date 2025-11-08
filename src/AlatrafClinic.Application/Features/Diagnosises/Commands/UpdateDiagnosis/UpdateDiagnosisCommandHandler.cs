using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Domain.Common.Results;
using AlatrafClinic.Domain.Diagnosises;
using AlatrafClinic.Domain.Diagnosises.Enums;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Inventory.Items;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Sales;
using AlatrafClinic.Domain.Services.Tickets;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;

using MediatR;

using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.UpdateDiagnosis;

public class UpdateDiagnosisCommandHandler : IRequestHandler<UpdateDiagnosisCommand, Result<Updated>>
{
    private readonly ILogger<UpdateDiagnosisCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _uow;

    public UpdateDiagnosisCommandHandler(ILogger<UpdateDiagnosisCommandHandler> logger, HybridCache cache, IUnitOfWork uow)
    {
        _logger = logger;
        _cache = cache;
        _uow = uow;
    }
    public async Task<Result<Updated>> Handle(UpdateDiagnosisCommand command, CancellationToken ct)
    {
        Diagnosis? diagnosis = await _uow.Diagnoses.GetByIdAsync(command.diagnosisId, ct);
        if (diagnosis is null)
        {
            _logger.LogWarning("Diagnosis with id {DiagnosisId} not found", command.diagnosisId);
            return DiagnosisErrors.DiagnosisNotFound;
        }
        Ticket? ticket = await _uow.Tickets.GetByIdAsync(command.ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with id {TicketId} not found", command.ticketId);
            return TicketErrors.TicketNotFound;
        }
        if (!ticket.IsEditable)
        {
            _logger.LogWarning("Ticket with id {TicketId} is read-only", command.ticketId);
            return TicketErrors.ReadOnly;
        }
        List<InjuryReason> injuryReasons = new();
        foreach (var reasonId in command.injuryReasons)
        {
            var reason = await _uow.InjuryReasons.GetByIdAsync(reasonId, ct);
            if (reason is not null)
            {
                injuryReasons.Add(reason);
            }
        }

        List<InjuryType> injuryTypes = new();
        foreach (var typeId in command.injuryTypes)
        {
            var type = await _uow.InjuryTypes.GetByIdAsync(typeId, ct);
            if (type is not null)
            {
                injuryTypes.Add(type);
            }
        }

        List<InjurySide> injurySides = new();
        foreach (var sideId in command.injurySides)
        {
            var side = await _uow.InjurySides.GetByIdAsync(sideId, ct);
            if (side is not null)
            {
                injurySides.Add(side);
            }
        }

        var updateResult = diagnosis.Update(
            command.diagnosisText,
            command.injuryDate,
            injuryReasons,
            injurySides,
            injuryTypes,
            command.diagnosisType);

        if (updateResult.IsError)
        {
            _logger.LogWarning("Failed to update diagnosis with id {DiagnosisId}: {Error}", command.diagnosisId, updateResult.TopError.Code);
            return updateResult;
        }

        Result<Updated> upsertResult;
        
        switch (command.diagnosisType)
        {
            case DiagnosisType.Therapy:
                {
                    upsertResult = await UpsertDiagnosisProgramsAsync(diagnosis, command.programs, ct);
                    if (upsertResult.IsError)
                    {
                        _logger.LogWarning("Failed to upsert diagnosis programs for patient {PatientId} with Ticket {TicketId}: {Error}", command.patientId, command.ticketId, upsertResult.Errors);
                        return upsertResult.Errors;
                    }
                }
                break;
            case DiagnosisType.Limbs:
                {
                    upsertResult = await UpsertDiagnosisIndustrialPartsAsync(diagnosis, command.industrialParts, ct);
                    if (upsertResult.IsError)
                    {
                        _logger.LogWarning("Failed to upsert diagnosis industrial parts for patient {PatientId} with Ticket {TicketId}: {Error}", command.patientId, command.ticketId, upsertResult.Errors);
                        return upsertResult.Errors;
                    }
                }
                break;
            case DiagnosisType.Sales:
                {
                    upsertResult = await UpsertDiagnosisItemsAsync(diagnosis, command.items, ct);
                    if (upsertResult.IsError)
                    {
                        _logger.LogWarning("Failed to upsert diagnosis sale items for patient {PatientId} with Ticket {TicketId}: {Error}", command.patientId, command.ticketId, upsertResult.Errors);
                        return upsertResult.Errors;
                    }
                }
                break;
            default:
                upsertResult = Error.Unexpected("Diagnosis.Type.Invalid", "The diagnosis type is invalid.");
                return upsertResult.Errors;
        }


        await _uow.Diagnoses.UpdateAsync(diagnosis, ct);
        await _uow.SaveChangesAsync(ct);

        return Result.Updated;
    }
    private async Task<Result<Updated>> UpsertDiagnosisProgramsAsync(Diagnosis diagnosis, List<(int medicalProgramId, int duration, string? notes)>? programs, CancellationToken ct)
    {
        if (programs is null || !programs.Any())
        {
            return DiagnosisErrors.MedicalProgramsAreRequired;
        }
        foreach (var (medicalProgramId, duration, notes) in programs)
        {
            var medicalProgram = await _uow.MedicalPrograms.IsExistAsync(medicalProgramId, ct);
            if (!medicalProgram)
            {
                _logger.LogWarning("Medical program with id {MedicalProgramId} not found", medicalProgramId);

                return MedicalProgramErrors.MedicalProgramNotFound;
            }
            diagnosis.UpsertDiagnosisPrograms(programs!);
        }

        return Result.Updated;
    }
    private async Task<Result<Updated>> UpsertDiagnosisIndustrialPartsAsync(Diagnosis diagnosis, List<(int partId, int unitId, int quantity, decimal price)>? industrialParts, CancellationToken ct)
    {
        if (industrialParts is null || !industrialParts.Any())
        {
            return DiagnosisErrors.IndustrialPartsAreRequired;
        }

        List<(int industrialPartUnitId, int quantity, decimal price)> incommingParts = new();

        foreach (var (partId, unitId, quantity, price) in industrialParts)
        {
            var industrialPartUnit = await _uow.IndustrialParts.GetByIdAndUnit(partId, unitId, ct);
            if (industrialPartUnit is null)
            {
                _logger.LogWarning("Industrial part unit with part id {PartId} and unit id {UnitId} not found", partId, unitId);

                return IndustrialPartUnitErrors.IndustrialPartUnitNotFound;
            }

            incommingParts.Add((industrialPartUnit.Id, quantity, price));
        }

        diagnosis.UpsertDiagnosisIndustrialParts(incommingParts);
        return Result.Updated;
    }

    private async Task<Result<Updated>> UpsertDiagnosisItemsAsync(Diagnosis diagnosis, List<(int itemId, int unitId, decimal quantity)>? items, CancellationToken ct)
    {
        if (items is null || !items.Any())
        {
            return DiagnosisErrors.SaleItemsAreRequired;
        }
        var sale = await _uow.Sales.GetByDiagnosisIdAsync(diagnosis.Id, ct);
        if (sale is null)
        {
            _logger.LogWarning("Sale for diagnosis id {DiagnosisId} not found", diagnosis.Id);
            return Error.NotFound("DiagnosisApplication.SaleNotFound", $"Sale not found for diagnosis {diagnosis.Id}");
        }
        
        List<(ItemUnit itemUnit, decimal quantity)> incommingItems = new();

        foreach (var (itemId, unitId, quantity) in items)
        {
            var itemUnit = await _uow.Items.GetByIdAndUnitIdAsync(itemId, unitId, ct);
            if (itemUnit is null)
            {
                _logger.LogWarning("Item unit with item id {ItemId} and unit id {UnitId} not found", itemId, unitId);

                return ItemUnitErrors.ItemUnitNotFound;
            }

            incommingItems.Add((itemUnit, quantity));
        }

        sale.UpsertItems(incommingItems);
        return Result.Updated;
    }
}