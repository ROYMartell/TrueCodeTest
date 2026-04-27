using Microsoft.EntityFrameworkCore;
using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Infrastructure.Persistence;

public sealed class EfRefreshTokenRepository(UserDbContext ctx) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenHashAsync(byte[] tokenHash, CancellationToken ct) =>
        ctx.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

    public async Task AddAsync(RefreshToken token, CancellationToken ct) =>
        await ctx.RefreshTokens.AddAsync(token, ct);

    public void Update(RefreshToken token) => ctx.RefreshTokens.Update(token);
}
