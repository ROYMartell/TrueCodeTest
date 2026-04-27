using MediatR;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Shared.Kernel.Abstractions;
using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Application.Errors;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Application.Commands.Login;

public sealed class LoginHandler(
    IUserRepository users,
    IRefreshTokenRepository refreshTokens,
    IPasswordHasher hasher,
    ITokenService tokens,
    IUnitOfWork uow,
    IDateTimeProvider clock)
    : IRequestHandler<LoginCommand, Result<TokenPair>>
{
    public async Task<Result<TokenPair>> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await users.GetByNameAsync(request.Name, ct);
        if (user is null || !hasher.Verify(request.Password, user.PasswordHash))
            return UserErrors.InvalidCredentials;

        var pair = tokens.CreatePair(user);

        await refreshTokens.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = pair.Refresh.Hash,
            ExpiresAt = pair.Refresh.ExpiresAt,
            CreatedAt = clock.UtcNow,
        }, ct);

        await uow.SaveChangesAsync(ct);
        return pair;
    }
}
