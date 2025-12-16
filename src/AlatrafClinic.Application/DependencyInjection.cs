using System.Reflection;
using FluentValidation;
using AlatrafClinic.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using AlatrafClinic.Application.Features.People.Services.CreatePerson;
using AlatrafClinic.Application.Features.People.Services.UpdatePerson;
using AlatrafClinic.Application.Features.Diagnosises.Services.CreateDiagnosis;
using AlatrafClinic.Application.Features.Diagnosises.Services.UpdateDiagnosis;

namespace AlatrafClinic.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            cfg.AddOpenBehavior(typeof(UnhandledExceptionBehaviour<,>));
            cfg.AddOpenBehavior(typeof(CachingBehavior<,>));
            cfg.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
        
        });
        services.AddScoped<IPersonCreateService, PersonCreateService>();
        services.AddScoped<IPersonUpdateService, PersonUpdateService>();
        services.AddScoped<IDiagnosisCreationService, DiagnosisCreationService>();
        services.AddScoped<IDiagnosisUpdateService, DiagnosisUpdateService>();
                
        return services;
    }
}

