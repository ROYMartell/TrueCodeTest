using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Application.Commands.RegisterUser;
using TrueCodeTest.Users.Application.Errors;
using TrueCodeTest.Users.Domain;
using TrueCodeTest.Users.UnitTests.Common;

namespace TrueCodeTest.Users.UnitTests.Commands;

public sealed class RegisterUserHandlerTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<IPasswordHasher> _hasher = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly FakeClock _clock = new();

    private RegisterUserHandler CreateSut() =>
        new(_users.Object, _hasher.Object, _uow.Object, _clock);

    [Fact]
    public async Task Register_HappyPath_ReturnsSuccessAndPersistsUser()
    {
        _users.Setup(r => r.ExistsByNameAsync("alice", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _hasher.Setup(h => h.Hash("secret")).Returns("HASH");

        var result = await CreateSut().Handle(new RegisterUserCommand("alice", "secret"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("alice");
        result.Value.PasswordHash.Should().Be("HASH");
        result.Value.Id.Should().NotBe(Guid.Empty);
        result.Value.CreatedAt.Should().Be(_clock.UtcNow);

        _users.Verify(r => r.AddAsync(It.Is<User>(u => u.Name == "alice" && u.PasswordHash == "HASH"), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Register_NameAlreadyTaken_ReturnsConflict()
    {
        _users.Setup(r => r.ExistsByNameAsync("alice", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await CreateSut().Handle(new RegisterUserCommand("alice", "secret"), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(UserErrors.NameTaken);
        _users.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
