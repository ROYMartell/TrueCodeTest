using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Application.Abstractions;

public sealed record AccessToken(string Token, DateTime ExpiresAt);
public sealed record RefreshTokenPayload(string Token, byte[] Hash, DateTime ExpiresAt);
public sealed record TokenPair(AccessToken Access, RefreshTokenPayload Refresh);

public interface ITokenService
{
    AccessToken CreateAccessToken(User user);
    RefreshTokenPayload CreateRefreshToken();
    byte[] HashRefreshToken(string refreshToken);
    TokenPair CreatePair(User user);
}
