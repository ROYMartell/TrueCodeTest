using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Application.Commands.Refresh;
using TrueCodeTest.Users.Application.Errors;
using TrueCodeTest.Users.Domain;
using TrueCodeTest.Users.UnitTests.Common;

namespace TrueCodeTest.Users.UnitTests.Commands;

public sealed class RefreshHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refresh = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly FakeClock _clock = new();

    private RefreshHandler CreateSut() =>
        new(_refresh.Object, _users.Object, _tokens.Object, _uow.Object, _clock);

    [Fact]
    public async Task Refresh_HappyPath_RotatesTokens()
    {
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, Name = "bob" };
        var existingHash = new byte[] { 1, 2, 3 };
        var stored = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = existingHash,
            ExpiresAt = _clock.UtcNow.AddDays(1),
            CreatedAt = _clock.UtcNow.AddDays(-1),
        };

        _tokens.Setup(t => t.HashRefreshToken("tok")).Returns(existingHash);
        _refresh.Setup(r => r.GetByTokenHashAsync(existingHash, It.IsAny<CancellationToken>())).ReturnsAsync(stored);
        _users.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var newPair = new TokenPair(
            new AccessToken("new-access", _clock.UtcNow.AddMinutes(15)),
            new RefreshTokenPayload("new-refresh", new byte[] { 9, 9 }, _clock.UtcNow.AddDays(7)));
        _tokens.Setup(t => t.CreatePair(user)).Returns(newPair);

        var result = await CreateSut().Handle(new RefreshCommand("tok"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        stored.RevokedAt.Should().Be(_clock.UtcNow);
        _refresh.Verify(r => r.Update(stored), Times.Once);
        _refresh.Verify(r => r.AddAsync(It.Is<RefreshToken>(t => t.TokenHash == newPair.Refresh.Hash), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Refresh_UnknownToken_ReturnsUnauthorized()
    {
        _tokens.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns(new byte[] { 0 });
        _refresh.Setup(r => r.GetByTokenHashAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).ReturnsAsync((RefreshToken?)null);

        var result = await CreateSut().Handle(new RefreshCommand("x"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.RefreshTokenNotFound);
    }

    [Fact]
    public async Task Refresh_RevokedToken_ReturnsUnauthorized()
    {
        var stored = new RefreshToken { RevokedAt = _clock.UtcNow.AddMinutes(-1), ExpiresAt = _clock.UtcNow.AddDays(1) };
        _tokens.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns(new byte[] { 0 });
        _refresh.Setup(r => r.GetByTokenHashAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(stored);

        var result = await CreateSut().Handle(new RefreshCommand("x"), CancellationToken.None);

        result.Error.Should().Be(UserErrors.RefreshTokenRevoked);
    }

    [Fact]
    public async Task Refresh_ExpiredToken_ReturnsUnauthorized()
    {
        var stored = new RefreshToken { ExpiresAt = _clock.UtcNow.AddMinutes(-1) };
        _tokens.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns(new byte[] { 0 });
        _refresh.Setup(r => r.GetByTokenHashAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(stored);

        var result = await CreateSut().Handle(new RefreshCommand("x"), CancellationToken.None);

        result.Error.Should().Be(UserErrors.RefreshTokenExpired);
    }
}
