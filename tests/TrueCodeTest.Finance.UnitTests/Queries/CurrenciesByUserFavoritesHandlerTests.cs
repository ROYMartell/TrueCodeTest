using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Application.Queries.CurrenciesByUserFavorites;
using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.UnitTests.Queries;

public sealed class CurrenciesByUserFavoritesHandlerTests
{
    private readonly Mock<IUserFavoriteRepository> _favorites = new();
    private readonly Mock<ICurrencyRepository> _currencies = new();

    private CurrenciesByUserFavoritesHandler CreateSut() => new(_favorites.Object, _currencies.Object);

    [Fact]
    public async Task NoFavorites_ReturnsEmpty()
    {
        var userId = Guid.NewGuid();
        _favorites.Setup(f => f.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<UserFavorite>());

        var result = await CreateSut().Handle(new CurrenciesByUserFavoritesQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
        _currencies.Verify(c => c.ListByIdsAsync(It.IsAny<IReadOnlyCollection<int>>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ReturnsCurrenciesForFavorites()
    {
        var userId = Guid.NewGuid();
        _favorites.Setup(f => f.ListByUserAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UserFavorite>
            {
                new() { UserId = userId, CurrencyId = 840 },
                new() { UserId = userId, CurrencyId = 978 },
            });

        var usd = new Currency { Id = 840, CharCode = "USD", Name = "Доллар США", Rate = 90m, Nominal = 1, UpdatedAt = DateTime.UtcNow };
        var eur = new Currency { Id = 978, CharCode = "EUR", Name = "Евро", Rate = 100m, Nominal = 1, UpdatedAt = DateTime.UtcNow };

        _currencies.Setup(c => c.ListByIdsAsync(
                It.Is<IReadOnlyCollection<int>>(ids => ids.Count == 2 && ids.Contains(840) && ids.Contains(978)),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Currency> { eur, usd });

        var result = await CreateSut().Handle(new CurrenciesByUserFavoritesQuery(userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Select(c => c.CharCode).Should().BeEquivalentTo(new[] { "EUR", "USD" });
    }
}
