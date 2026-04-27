using Microsoft.EntityFrameworkCore;
using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.Infrastructure.Persistence;

public sealed class EfCurrencyRepository(FinanceDbContext ctx) : ICurrencyRepository
{
    public async Task<IReadOnlyList<Currency>> ListAllAsync(CancellationToken ct) =>
        await ctx.Currencies.AsNoTracking().OrderBy(c => c.CharCode).ToListAsync(ct);

    public Task<Currency?> GetByIdAsync(int id, CancellationToken ct) =>
        ctx.Currencies.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        ctx.Currencies.AnyAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Currency>> ListByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct)
    {
        if (ids.Count == 0)
            return Array.Empty<Currency>();
        return await ctx.Currencies
            .AsNoTracking()
            .Where(c => ids.Contains(c.Id))
            .OrderBy(c => c.CharCode)
            .ToListAsync(ct);
    }
}
