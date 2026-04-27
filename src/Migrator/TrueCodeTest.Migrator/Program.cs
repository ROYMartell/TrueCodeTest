using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TrueCodeTest.Finance.Infrastructure.Persistence;
using TrueCodeTest.Users.Infrastructure.Persistence;

var builder = Host.CreateApplicationBuilder(args);

var usersConnection = builder.Configuration.GetConnectionString("UsersDb")
    ?? throw new InvalidOperationException("ConnectionStrings:UsersDb is not configured");
var financeConnection = builder.Configuration.GetConnectionString("FinanceDb")
    ?? throw new InvalidOperationException("ConnectionStrings:FinanceDb is not configured");

builder.Services.AddDbContext<UserDbContext>(o => o.UseNpgsql(usersConnection));
builder.Services.AddDbContext<FinanceDbContext>(o => o.UseNpgsql(financeConnection));

using var host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

try
{
    await using var scope = host.Services.CreateAsyncScope();
    var users = scope.ServiceProvider.GetRequiredService<UserDbContext>();
    var finance = scope.ServiceProvider.GetRequiredService<FinanceDbContext>();

    logger.LogInformation("Applying users_db migrations...");
    await users.Database.MigrateAsync();

    logger.LogInformation("Applying finance_db migrations...");
    await finance.Database.MigrateAsync();

    logger.LogInformation("Migrations applied successfully");
    return 0;
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Migration failed");
    return 1;
}

public partial class Program { }
