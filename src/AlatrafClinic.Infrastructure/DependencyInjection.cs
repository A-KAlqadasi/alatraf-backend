using System.Data;
using System.Text;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Events;
using AlatrafClinic.Application.Common.Interfaces.Repositories;

using AlatrafClinic.Application.Reports.Dtos;
using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Application.Reports.Validators;
using AlatrafClinic.Infrastructure.Data;
using AlatrafClinic.Infrastructure.Data.Inbox;
using AlatrafClinic.Infrastructure.Data.Interceptors;
using AlatrafClinic.Infrastructure.Data.Repositories;
using AlatrafClinic.Infrastructure.Identity;
using AlatrafClinic.Infrastructure.Reports;
using AlatrafClinic.Infrastructure.Reports.Helpers;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using AlatrafClinic.Application.Sagas;
using AlatrafClinic.Application.Sagas.Compensation;
using AlatrafClinic.Infrastructure.Eventing;
using AlatrafClinic.Infrastructure.Messaging;
using AlatrafClinic.Application.Common.Interfaces.Messaging;


namespace AlatrafClinic.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        ArgumentNullException.ThrowIfNull(connectionString);

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

        services.AddDbContext<AlatrafClinicDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
        });



        // services.AddScoped<ApplicationDbContextInitialiser>();
        services.AddScoped<IAppDbContext>(provider => provider.GetRequiredService<AlatrafClinicDbContext>());
        services.AddScoped<AlatrafClinicDbContextInitialiser>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var jwtSettings = configuration.GetSection("JwtSettings");

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(jwtSettings["Secret"]!)),
            };
        });

        services
        .AddIdentityCore<AppUser>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 1;
            options.SignIn.RequireConfirmedAccount = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AlatrafClinicDbContext>()
        .AddDefaultTokenProviders();

        // services.AddScoped<IAuthorizationHandler, LaborAssignedHandler>();

        // services.AddAuthorizationBuilder()
        //       .AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"))
        //       .AddPolicy("SelfScopedWorkOrderAccess", policy =>
        //         policy.Requirements.Add(new LaborAssignedRequirement()));

        services.AddTransient<IIdentityService, IdentityService>();


        services.AddHybridCache(options => options.DefaultEntryOptions = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(10), // L2, L3
            LocalCacheExpiration = TimeSpan.FromSeconds(30), // L1
        });

        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        // EventContext implementation lives in Infrastructure (concrete moved)
        services.AddScoped<IEventContext, EventContext>();
        // Register Domain Events dispatcher behavior
        services.AddTransient(typeof(MediatR.IPipelineBehavior<,>), typeof(DomainEventsDispatcherBehavior<,>));
        // Messaging transport (No-op by default)
        services.AddScoped<IMessagePublisher, NoopMessagePublisher>();
        services.AddHostedService<OutboxProcessor>();
        services.AddScoped<IInbox, Inbox>();
        services.AddScoped<IIdempotencyContext, IdempotencyContext>();


        // Dapper Services
        services.AddReportServices(configuration);

        services.AddScoped<ISagaStateService, SagaStateService>();
        services.AddScoped<ISagaCompensationHandler, SaleSagaCompensationHandler>();
        services.AddScoped<IEnumerable<ISagaCompensationHandler>>(sp =>
            sp.GetServices<ISagaCompensationHandler>().ToList());
        return services;
    }

    public static IServiceCollection AddReportServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Core services
        services.AddScoped<IReportMetadataRepository, ReportMetadataRepository>();
        services.AddScoped<IReportSqlBuilder, ReportSqlBuilder>();
        services.AddScoped<IReportQueryExecutor, DapperReportQueryExecutor>();
        services.AddScoped<IReportService, ReportService>();

        // Infrastructure
        services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddSingleton<ISqlDialect>(GetSqlDialect(configuration));

        // Validation
        services.AddScoped<IValidator<ReportRequestDto>, ReportRequestValidator>();
        services.AddScoped<IValidator<ReportFilterDto>, ReportFilterValidator>();
        services.AddScoped<IValidator<ReportSortDto>, ReportSortValidator>();

        // Caching
        services.AddHybridCache(options =>
        {
            options.MaximumPayloadBytes = 1024 * 1024; // 1MB
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(30)
            };
        });

        services.AddScoped<IReportExportService, ReportExportService>();


        return services;
    }

    private static ISqlDialect GetSqlDialect(IConfiguration configuration)
    {
        var databaseType = configuration["Database:Type"]?.ToLower() ?? "sqlserver";

        return databaseType switch
        {
            "postgresql" or "postgres" => new PostgreSqlDialect(),
            "mysql" => new MySqlDialect(),
            "sqlite" => new SqlServerDialect(), // SQLite uses similar syntax
            _ => new SqlServerDialect()
        };
    }
}