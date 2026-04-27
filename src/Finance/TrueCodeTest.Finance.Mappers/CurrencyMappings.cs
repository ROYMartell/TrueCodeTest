using TrueCodeTest.Finance.Contracts.Currencies;
using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.Mappers;

public static class CurrencyMappings
{
    private static CurrencyDto ToDto(this Currency currency) => new(
        currency.Id,
        currency.CharCode,
        currency.Name,
        currency.Rate,
        currency.Nominal,
        currency.UpdatedAt);

    public static IReadOnlyList<CurrencyDto> ToDto(this IReadOnlyList<Currency> currencies) =>
        currencies.Select(ToDto).ToList();
}
