using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Infrastructure.Persistence;

namespace TrueCodeTest.Finance.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddFinanceInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("FinanceDb")
            ?? throw new InvalidOperationException("ConnectionStrings:FinanceDb is not configured");

        services.AddDbContext<FinanceDbContext>(options => options.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__ef_migrations_history", "public");
        }));

        services.AddScoped<ICurrencyRepository, EfCurrencyRepository>();
        services.AddScoped<IUserFavoriteRepository, EfUserFavoriteRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        return services;
    }
}
