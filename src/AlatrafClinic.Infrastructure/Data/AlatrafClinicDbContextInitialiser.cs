using AlatrafClinic.Domain.Departments;
using AlatrafClinic.Domain.Services;

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