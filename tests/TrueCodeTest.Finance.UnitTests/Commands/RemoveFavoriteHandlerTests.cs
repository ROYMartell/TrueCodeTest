using TrueCodeTest.Finance.Application.Abstractions;
using TrueCodeTest.Finance.Application.Commands.RemoveFavorite;
using TrueCodeTest.Finance.Application.Errors;
using TrueCodeTest.Finance.Domain;

namespace TrueCodeTest.Finance.UnitTests.Commands;

public sealed class RemoveFavoriteHandlerTests
{
    private readonly Mock<IUserFavoriteRepository> _favorites = new();
    private readonly Mock<IUnitOfWork> _uow = new();

    private RemoveFavoriteHandler CreateSut() => new(_favorites.Object, _uow.Object);

    [Fact]
    public async Task Remove_HappyPath_DeletesFavorite()
    {
        var userId = Guid.NewGuid();
        var fav = new UserFavorite { UserId = userId, CurrencyId = 840 };
        _favorites.Setup(f => f.FindAsync(userId, 840, It.IsAny<CancellationToken>())).ReturnsAsync(fav);

        var result = await CreateSut().Handle(new RemoveFavoriteCommand(userId, 840), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _favorites.Verify(f => f.Remove(fav), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Remove_NotFound_ReturnsNotFound()
    {
        _favorites.Setup(f => f.FindAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((UserFavorite?)null);

        var result = await CreateSut().Handle(new RemoveFavoriteCommand(Guid.NewGuid(), 1), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(FinanceErrors.FavoriteNotFound);
    }
}
