using MediatR;
using TrueCodeTest.Shared.Kernel;
using TrueCodeTest.Shared.Kernel.Abstractions;
using TrueCodeTest.Users.Application.Abstractions;

namespace TrueCodeTest.Users.Application.Commands.Logout;

public sealed class LogoutHandler(
    IRefreshTokenRepository refreshTokens,
    ITokenService tokens,
    IUnitOfWork uow,
    IDateTimeProvider clock)
    : IRequestHandler<LogoutCommand, Result>
{
    public async Task<Result> Handle(LogoutCommand request, CancellationToken ct)
    {
        var hash = tokens.HashRefreshToken(request.RefreshToken);
        var stored = await refreshTokens.GetByTokenHashAsync(hash, ct);
        if (stored is null || stored.RevokedAt is not null)
            return Result.Success();

        stored.RevokedAt = clock.UtcNow;
        refreshTokens.Update(stored);
        await uow.SaveChangesAsync(ct);
        return Result.Success();
    }
}
