using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<User?> GetByNameAsync(string name, CancellationToken ct);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct);
    Task AddAsync(User user, CancellationToken ct);
}
