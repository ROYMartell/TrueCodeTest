using TrueCodeTest.Users.Application.Abstractions;

namespace TrueCodeTest.Users.Infrastructure.Persistence;

public sealed class EfUnitOfWork(UserDbContext ctx) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => ctx.SaveChangesAsync(ct);
}
