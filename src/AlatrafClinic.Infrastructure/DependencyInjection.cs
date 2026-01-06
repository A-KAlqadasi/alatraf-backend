using System.Data;
using System.Text;

using AlatrafClinic.Application.Common.Interfaces;
using AlatrafClinic.Application.Common.Interfaces.Repositories;
using AlatrafClinic.Application.Reports.Interfaces;
using AlatrafClinic.Application.Reports.Services;
using AlatrafClinic.Infrastructure.Data;
using AlatrafClinic.Infrastructure.Data.Interceptors;
using AlatrafClinic.Infrastructure.Data.Repositories;
using AlatrafClinic.Infrastructure.Identity;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


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

        services.AddScoped<IAppDbContext>(provider => provider. GetRequiredService<AlatrafClinicDbContext>());
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

        // Dapper Services
        services.AddScoped<IDbConnection>(sp =>
        new SqlConnection(connectionString));

        services.AddScoped<IReportMetadataRepository, ReportMetadataRepository>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IReportSqlBuilder, ReportSqlBuilder>();
        services.AddScoped<IReportQueryExecutor, DapperReportQueryExecutor>();

        return services;
    }
}