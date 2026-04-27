using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TrueCodeTest.Users.Infrastructure.Persistence;

public sealed class DesignTimeUserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    public UserDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=users_db;Username=postgres;Password=postgres")
            .Options;
        return new UserDbContext(options);
    }
}
