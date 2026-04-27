using Microsoft.EntityFrameworkCore;
using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.Infrastructure.Persistence;

public sealed class EfUserFavoriteRepository(FinanceDbContext ctx) : IUserFavoriteRepository
{
    public async Task<IReadOnlyList<UserFavorite>> ListByUserAsync(Guid userId, CancellationToken ct) =>
        await ctx.UserFavorites.AsNoTracking().Where(f => f.UserId == userId).ToListAsync(ct);

    public Task<UserFavorite?> FindAsync(Guid userId, int currencyId, CancellationToken ct) =>
        ctx.UserFavorites.FirstOrDefaultAsync(f => f.UserId == userId && f.CurrencyId == currencyId, ct);

    public async Task AddAsync(UserFavorite favorite, CancellationToken ct) =>
        await ctx.UserFavorites.AddAsync(favorite, ct);

    public void Remove(UserFavorite favorite) => ctx.UserFavorites.Remove(favorite);
}
