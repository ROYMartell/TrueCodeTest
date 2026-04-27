using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Application.Commands.AddFavorite;
using TrueCodeTest.Finance.Application.Errors;
using TrueCodeTest.Finance.Domain;
using TrueCodeTest.Finance.UnitTests.Common;

namespace TrueCodeTest.Finance.UnitTests.Commands;

public sealed class AddFavoriteHandlerTests
{
    private readonly Mock<IUserFavoriteRepository> _favorites = new();
    private readonly Mock<ICurrencyRepository> _currencies = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly FakeClock _clock = new();

    private AddFavoriteHandler CreateSut() =>
        new(_favorites.Object, _currencies.Object, _uow.Object, _clock);

    [Fact]
    public async Task Add_HappyPath_PersistsFavorite()
    {
        var userId = Guid.NewGuid();
        _currencies.Setup(c => c.ExistsAsync(840, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _favorites.Setup(f => f.FindAsync(userId, 840, It.IsAny<CancellationToken>())).ReturnsAsync((UserFavorite?)null);

        var result = await CreateSut().Handle(new AddFavoriteCommand(userId, 840), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _favorites.Verify(f => f.AddAsync(It.Is<UserFavorite>(x =>
            x.UserId == userId && x.CurrencyId == 840 && x.CreatedAt == _clock.UtcNow), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Add_CurrencyNotFound_ReturnsNotFound()
    {
        _currencies.Setup(c => c.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var result = await CreateSut().Handle(new AddFavoriteCommand(Guid.NewGuid(), 999), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FinanceErrors.CurrencyNotFound);
        _favorites.Verify(f => f.AddAsync(It.IsAny<UserFavorite>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Add_AlreadyExists_ReturnsConflict()
    {
        var userId = Guid.NewGuid();
        _currencies.Setup(c => c.ExistsAsync(840, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _favorites.Setup(f => f.FindAsync(userId, 840, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new UserFavorite { UserId = userId, CurrencyId = 840 });

        var result = await CreateSut().Handle(new AddFavoriteCommand(userId, 840), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FinanceErrors.FavoriteAlreadyExists);
    }
}
