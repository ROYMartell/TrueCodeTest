using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.Application.Abstractions;

public interface IUserFavoriteRepository
{
    Task<IReadOnlyList<UserFavorite>> ListByUserAsync(Guid userId, CancellationToken ct);
    Task<UserFavorite?> FindAsync(Guid userId, int currencyId, CancellationToken ct);
    Task AddAsync(UserFavorite favorite, CancellationToken ct);
    void Remove(UserFavorite favorite);
}
