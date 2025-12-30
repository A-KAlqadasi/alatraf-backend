using AlatrafClinic.Domain.Common.Constants;
using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Diagnosises.InjuryReasons;
using AlatrafClinic.Domain.Diagnosises.InjurySides;
using AlatrafClinic.Domain.Diagnosises.InjuryTypes;
using AlatrafClinic.Domain.Inventory.Units;
using AlatrafClinic.Domain.RepairCards.IndustrialParts;
using AlatrafClinic.Domain.Services;
using AlatrafClinic.Domain.Settings;
using AlatrafClinic.Domain.TherapyCards.Enums;
using AlatrafClinic.Domain.TherapyCards.MedicalPrograms;
using AlatrafClinic.Domain.TherapyCards.TherapyCardTypePrices;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AlatrafClinic.Infrastructure.Data;

public sealed class AlatrafClinicDbContextInitialiser
{
    private readonly ILogger<AlatrafClinicDbContextInitialiser> _logger;
    private readonly AlatrafClinicDbContext _context;
    
    public AlatrafClinicDbContextInitialiser(
        ILogger<AlatrafClinicDbContextInitialiser> logger,
        AlatrafClinicDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync(CancellationToken ct = default)
    {
        try
        {
            await _context.Database.EnsureCreatedAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }
    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }
    private async Task TrySeedAsync()
    {
        if(!await _context.Departments.AnyAsync())
        {
            _context.Departments.AddRange(
                Department.Create("العلاج الطبيعي").Value,
                Department.Create("الادارة الفنية").Value
            );
            
        }

        // Seed, if necessary
        if (!await _context.Services.AnyAsync())
        {
            _context.Services.AddRange(
                Service.Create("استشارة", null).Value,
                Service.Create("علاج طبيعي", null).Value,
                Service.Create("اطراف صناعية", null).Value,
                Service.Create("مبيعات", null).Value,
                Service.Create("إصلاحات", null).Value,
                Service.Create("عظام", null).Value,
                Service.Create("أعصاب", null).Value,
                Service.Create("تجديد كروت علاج", null).Value,
                Service.Create("إصدار بدل فاقد لكرت علاج", null, price: 500).Value
            );

        }
        if (!await _context.InjuryTypes.AnyAsync())
        {
            _context.InjuryTypes.AddRange(
                InjuryType.Create("كسر يد").Value,
                InjuryType.Create("حرق").Value,
                InjuryType.Create("كسر قدم").Value
            );
        }
        if (!await _context.InjuryReasons.AnyAsync())
        {
            _context.InjuryReasons.AddRange(
                InjuryReason.Create("حادث").Value,
                InjuryReason.Create("إجهاد").Value,
                InjuryReason.Create("مرض").Value
            );
        }
        if (!await _context.InjurySides.AnyAsync())
        {
            _context.InjurySides.AddRange(
                InjurySide.Create("اليد المينى").Value,
                InjurySide.Create("اليد اليسرى").Value,
                InjurySide.Create("الرجل اليمنى").Value,
                InjurySide.Create("الرجل اليسرى").Value
            );
        }
        if (!await _context.TherapyCardTypePrices.AnyAsync())
        {
            _context.TherapyCardTypePrices.AddRange(
                TherapyCardTypePrice.Create(TherapyCardType.General, 200m).Value,
                TherapyCardTypePrice.Create(TherapyCardType.Special, 2000m).Value,
                TherapyCardTypePrice.Create(TherapyCardType.NerveKids, 400m).Value
            );
        }

        if (!await _context.Units.AnyAsync())
        {
            _context.Units.AddRange(
                GeneralUnit.Create("قطعة").Value,
                GeneralUnit.Create("زوج").Value,
                GeneralUnit.Create("طرف علوي يمين").Value,
                GeneralUnit.Create("طرف علوي يسار").Value,
                GeneralUnit.Create("طرف سفلي يمين").Value,
                GeneralUnit.Create("طرف سفلي يسار").Value,
                GeneralUnit.Create("الطرفين العلويين").Value,
                GeneralUnit.Create("الطرفين السفليين").Value
            );
        }
        
        if (!await _context.MedicalPrograms.AnyAsync())
        {
            _context.MedicalPrograms.AddRange(
                MedicalProgram.Create("تمارين").Value,
                MedicalProgram.Create("مساج").Value,
                MedicalProgram.Create("حرارة").Value
            );
        }

        if (!await _context.IndustrialParts.AnyAsync())
        {
            _context.IndustrialParts.AddRange(
                IndustrialPart.Create("طرف صناعي").Value,
                IndustrialPart.Create("رجل صناعي").Value,
                IndustrialPart.Create("دعامة ركبة").Value
            );
        }
        if (!await _context.AppSettings.AnyAsync())
        {
            _context.AppSettings.AddRange(
                AppSetting.Create(AlatrafClinicConstants.AllowedDaysKey, "Saturday, Tuesday").Value,
                AppSetting.Create(AlatrafClinicConstants.AppointmentDailyCapacityKey, AlatrafClinicConstants.DefaultAppointmentDailyCapacity.ToString()).Value,
                AppSetting.Create(AlatrafClinicConstants.WoundedReportMinTotalKey, "30000").Value
            );
        }
        
        await _context.SaveChangesAsync();
    }
}

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<AlatrafClinicDbContextInitialiser>();

        await initialiser.InitialiseAsync();
        await initialiser.SeedAsync();
    }
}