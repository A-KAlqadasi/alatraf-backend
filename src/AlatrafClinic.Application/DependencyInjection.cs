using System.Reflection;
using FluentValidation;
using AlatrafClinic.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;
using AlatrafClinic.Application.Features.People.Persons.Services;
using AlatrafClinic.Application.Features.People.Persons.Services.UpdatePerson;

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
        });
        services.AddScoped<IPersonCreateService, PersonCreateService>();
        services.AddScoped<IPersonUpdateService, PersonUpdateService>();

        // 🧩 Validators
        services.AddScoped<IValidator<PersonInput>, PersonInputValidator>();

        return services;
    }
}

