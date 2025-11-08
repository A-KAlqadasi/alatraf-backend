using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Features.Diagnosises.Dtos;
using AlatrafClinic.Application.Features.Diagnosises.Mappers;
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

namespace AlatrafClinic.Application.Features.Diagnosises.Commands.CreateDiagnosis;

public class CreateDiagnosisCommandHandler : IRequestHandler<CreateDiagnosisCommand, Result<DiagnosisDto>>
{
    private readonly ILogger<CreateDiagnosisCommandHandler> _logger;
    private readonly HybridCache _cache;
    private readonly IUnitOfWork _uow;

    public CreateDiagnosisCommandHandler(ILogger<CreateDiagnosisCommandHandler> logger, HybridCache cache, IUnitOfWork uow)
    {
        _logger = logger;
        _cache = cache;
        _uow = uow;
    }
    public async Task<Result<DiagnosisDto>> Handle(CreateDiagnosisCommand command, CancellationToken ct)
    {
        Ticket? ticket = await _uow.Tickets.GetByIdAsync(command.ticketId, ct);
        if (ticket is null)
        {
            _logger.LogWarning("Ticket with id {TicketId} not found", command.ticketId);
            return TicketErrors.TicketNotFound;
        }
        if (!ticket.IsEditable)
        {
            _logger.LogWarning("Ticket with id {TicketId} is not editable", command.ticketId);
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
        var diagnosisResult = Diagnosis.Create(
            command.ticketId,
            command.diagnosisText,
            command.injuryDate,
            injuryReasons,
            injurySides,
            injuryTypes,
            command.patientId,
            command.diagnosisType
        );

        if (diagnosisResult.IsError)
        {
            _logger.LogWarning("Failed to create diagnosis for ticket {TicketId}: {Error}", command.ticketId, diagnosisResult.Errors);
            return diagnosisResult.Errors;
        }
        var diagnosis = diagnosisResult.Value;

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

        await _uow.Diagnoses.AddAsync(diagnosis, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Diagnosis created successfully with Id {DiagnosisId} for ticket {TicketId}", diagnosis.Id, command.ticketId);

        return diagnosis.ToDto();
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

        var saleResult = Sale.Create(diagnosis.Id);
        if (saleResult.IsError)
        {
            return saleResult.Errors;
        }
        var sale = saleResult.Value;
        sale.UpsertItems(incommingItems);
        diagnosis.AssignToSale(sale);
        
        return Result.Updated;
    }
}