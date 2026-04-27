using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Application.Commands.Login;
using TrueCodeTest.Users.Application.Errors;
using TrueCodeTest.Users.Domain;
using TrueCodeTest.Users.UnitTests.Common;

namespace TrueCodeTest.Users.UnitTests.Commands;

public sealed class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IRefreshTokenRepository> _refresh = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly FakeClock _clock = new();

    private LoginHandler CreateSut() =>
        new(_users.Object, _refresh.Object, _hasher.Object, _tokens.Object, _uow.Object, _clock);

    private static User MakeUser() => new() { Id = Guid.NewGuid(), Name = "alice", PasswordHash = "HASH" };

    [Fact]
    public async Task Login_Success_ReturnsTokensAndPersistsRefresh()
    {
        var user = MakeUser();
        _users.Setup(r => r.GetByNameAsync("alice", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify("secret", user.PasswordHash)).Returns(true);

        var expected = new TokenPair(
            new AccessToken("access-jwt", _clock.UtcNow.AddMinutes(15)),
            new RefreshTokenPayload("refresh-str", new byte[] { 1, 2, 3 }, _clock.UtcNow.AddDays(7)));
        _tokens.Setup(t => t.CreatePair(user)).Returns(expected);

        var result = await CreateSut().Handle(new LoginCommand("alice", "secret"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Access.Token.Should().Be("access-jwt");
        result.Value.Refresh.Token.Should().Be("refresh-str");

        _refresh.Verify(r => r.AddAsync(It.Is<RefreshToken>(
            rt => rt.UserId == user.Id && rt.TokenHash == expected.Refresh.Hash),
            It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Login_UnknownUser_ReturnsUnauthorized()
    {
        _users.Setup(r => r.GetByNameAsync("nobody", It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var result = await CreateSut().Handle(new LoginCommand("nobody", "whatever"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var user = MakeUser();
        _users.Setup(r => r.GetByNameAsync("alice", It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _hasher.Setup(h => h.Verify(It.IsAny<string>(), user.PasswordHash)).Returns(false);

        var result = await CreateSut().Handle(new LoginCommand("alice", "bad"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.InvalidCredentials);
        _refresh.Verify(r => r.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
