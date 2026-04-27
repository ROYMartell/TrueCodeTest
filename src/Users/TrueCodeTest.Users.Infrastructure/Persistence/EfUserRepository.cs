using Microsoft.EntityFrameworkCore;
using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Infrastructure.Persistence;

public sealed class EfUserRepository(UserDbContext ctx) : IUserRepository
{
    public Task<User?> GetByIdAsync(Guid id, CancellationToken ct) =>
        ctx.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> GetByNameAsync(string name, CancellationToken ct) =>
        ctx.Users.FirstOrDefaultAsync(u => u.Name == name, ct);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct) =>
        ctx.Users.AnyAsync(u => u.Name == name, ct);

    public async Task AddAsync(User user, CancellationToken ct) =>
        await ctx.Users.AddAsync(user, ct);
}
