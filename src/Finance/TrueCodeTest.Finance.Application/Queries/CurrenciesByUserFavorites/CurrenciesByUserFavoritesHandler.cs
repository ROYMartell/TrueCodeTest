using MediatR;
using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Domain;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Queries.CurrenciesByUserFavorites;

public sealed class CurrenciesByUserFavoritesHandler(
    IUserFavoriteRepository favorites,
    ICurrencyRepository currencies)
    : IRequestHandler<CurrenciesByUserFavoritesQuery, Result<IReadOnlyList<Currency>>>
{
    public async Task<Result<IReadOnlyList<Currency>>> Handle(
        CurrenciesByUserFavoritesQuery request,
        CancellationToken ct)
    {
        var favorites1 = await favorites.ListByUserAsync(request.UserId, ct);
        if (favorites1.Count == 0)
            return Result.Success<IReadOnlyList<Currency>>(Array.Empty<Currency>());

        var ids = favorites1.Select(f => f.CurrencyId).ToArray();
        var currencies1 = await currencies.ListByIdsAsync(ids, ct);
        return Result.Success(currencies1);
    }
}
