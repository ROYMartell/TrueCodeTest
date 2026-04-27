using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TrueCodeTest.Finance.Infrastructure.Persistence;

public sealed class DesignTimeFinanceDbContextFactory : IDesignTimeDbContextFactory<FinanceDbContext>
{
    public FinanceDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<FinanceDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=finance_db;Username=postgres;Password=postgres")
            .Options;
        return new FinanceDbContext(options);
    }
}
