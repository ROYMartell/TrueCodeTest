namespace TrueCodeTest.Finance.Contracts.Currencies;

public sealed record CurrencyDto(
    int Id,
    string CharCode,
    string Name,
    decimal Rate,
    int Nominal,
    DateTime UpdatedAt);
