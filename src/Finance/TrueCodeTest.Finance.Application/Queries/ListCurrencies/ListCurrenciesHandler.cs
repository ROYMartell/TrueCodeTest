using MediatR;
using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Domain;
using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Queries.ListCurrencies;

public sealed class ListCurrenciesHandler(ICurrencyRepository currencies)
    : IRequestHandler<ListCurrenciesQuery, Result<IReadOnlyList<Currency>>>
{
    public async Task<Result<IReadOnlyList<Currency>>> Handle(ListCurrenciesQuery request, CancellationToken ct)
    {
        var items = await currencies.ListAllAsync(ct);
        return Result.Success(items);
    }
}
