using MediatR;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Shared.Kernel.Abstractions;
using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Application.Errors;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Application.Commands.Refresh;

public sealed class RefreshHandler(
    IRefreshTokenRepository refreshTokens,
    IUserRepository users,
    ITokenService tokens,
    IUnitOfWork uow,
    IDateTimeProvider clock)
    : IRequestHandler<RefreshCommand, Result<TokenPair>>
{
    public async Task<Result<TokenPair>> Handle(RefreshCommand request, CancellationToken ct)
    {
        var hash = tokens.HashRefreshToken(request.RefreshToken);
        var stored = await refreshTokens.GetByTokenHashAsync(hash, ct);
        if (stored is null)
            return UserErrors.RefreshTokenNotFound;

        var now = clock.UtcNow;
        if (stored.RevokedAt is not null)
            return UserErrors.RefreshTokenRevoked;
        if (stored.ExpiresAt <= now)
            return UserErrors.RefreshTokenExpired;

        var user = await users.GetByIdAsync(stored.UserId, ct);
        if (user is null)
            return UserErrors.RefreshTokenNotFound;

        stored.RevokedAt = now;
        refreshTokens.Update(stored);

        var pair = tokens.CreatePair(user);
        await refreshTokens.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = pair.Refresh.Hash,
            ExpiresAt = pair.Refresh.ExpiresAt,
            CreatedAt = now,
        }, ct);

        await uow.SaveChangesAsync(ct);
        return pair;
    }
}
