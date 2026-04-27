using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Application.Commands.Logout;
using TrueCodeTest.Users.Domain;
using TrueCodeTest.Users.UnitTests.Common;

namespace TrueCodeTest.Users.UnitTests.Commands;

public sealed class LogoutHandlerTests
{
    private readonly Mock<IRefreshTokenRepository> _refresh = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly FakeClock _clock = new();

    private LogoutHandler CreateSut() =>
        new(_refresh.Object, _tokens.Object, _uow.Object, _clock);

    [Fact]
    public async Task Logout_Success_RevokesToken()
    {
        var hash = new byte[] { 1, 2 };
        var stored = new RefreshToken { ExpiresAt = _clock.UtcNow.AddDays(1), TokenHash = hash };
        _tokens.Setup(t => t.HashRefreshToken("tok")).Returns(hash);
        _refresh.Setup(r => r.GetByTokenHashAsync(hash, It.IsAny<CancellationToken>())).ReturnsAsync(stored);

        var result = await CreateSut().Handle(new LogoutCommand("tok"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        stored.RevokedAt.Should().Be(_clock.UtcNow);
        _refresh.Verify(r => r.Update(stored), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Logout_UnknownToken_IsNoOp()
    {
        _tokens.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns(new byte[] { 0 });
        _refresh.Setup(r => r.GetByTokenHashAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).ReturnsAsync((RefreshToken?)null);

        var result = await CreateSut().Handle(new LogoutCommand("x"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _refresh.Verify(r => r.Update(It.IsAny<RefreshToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Logout_AlreadyRevoked_IsNoOp()
    {
        var stored = new RefreshToken { RevokedAt = _clock.UtcNow.AddMinutes(-5) };
        _tokens.Setup(t => t.HashRefreshToken(It.IsAny<string>())).Returns(new byte[] { 0 });
        _refresh.Setup(r => r.GetByTokenHashAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>())).ReturnsAsync(stored);

        var result = await CreateSut().Handle(new LogoutCommand("x"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _refresh.Verify(r => r.Update(It.IsAny<RefreshToken>()), Times.Never);
    }
}
