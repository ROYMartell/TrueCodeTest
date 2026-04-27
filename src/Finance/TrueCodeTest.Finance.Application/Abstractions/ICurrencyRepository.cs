using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.Application.Abstractions;

public interface ICurrencyRepository
{
    Task<IReadOnlyList<Currency>> ListAllAsync(CancellationToken ct);
    Task<Currency?> GetByIdAsync(int id, CancellationToken ct);
    Task<bool> ExistsAsync(int id, CancellationToken ct);
    Task<IReadOnlyList<Currency>> ListByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken ct);
}
