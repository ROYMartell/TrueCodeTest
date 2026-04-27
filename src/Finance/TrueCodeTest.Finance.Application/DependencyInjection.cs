using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeTest.Shared.Kernel.Abstractions;
using TrueCodeTest.Shared.Mediator;

namespace TrueCodeTest.Finance.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddFinanceApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(assembly);
        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

        return services;
    }
}
