using TrueCodeTest.Finance.Application.Abstractions;

namespace TrueCodeTest.Finance.Infrastructure.Persistence;

public sealed class EfUnitOfWork(FinanceDbContext ctx) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => ctx.SaveChangesAsync(ct);
}
