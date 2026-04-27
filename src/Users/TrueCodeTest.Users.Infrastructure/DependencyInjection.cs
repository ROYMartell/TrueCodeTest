using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TrueCodeTest.Shared.Auth;
using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Infrastructure.Persistence;
using TrueCodeTest.Users.Infrastructure.Security;

namespace TrueCodeTest.Users.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddUsersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("UsersDb")
            ?? throw new InvalidOperationException("ConnectionStrings:UsersDb is not configured");

        services.AddDbContext<UserDbContext>(options => options.UseNpgsql(connectionString, npgsql =>
        {
            npgsql.MigrationsHistoryTable("__ef_migrations_history", "public");
        }));

        services.AddScoped<IUserRepository, EfUserRepository>();
        services.AddScoped<IRefreshTokenRepository, EfRefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<ITokenService, JwtTokenService>();

        return services;
    }
}
