using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Finance.Application.Errors;

public static class FinanceErrors
{
    public static readonly Error CurrencyNotFound = Error.NotFound(
        "finance.currency_not_found",
        "Currency not found");

    public static readonly Error FavoriteAlreadyExists = Error.Conflict(
        "finance.favorite_exists",
        "Currency is already in favorites");

    public static readonly Error FavoriteNotFound = Error.NotFound(
        "finance.favorite_not_found",
        "Favorite not found");
}
