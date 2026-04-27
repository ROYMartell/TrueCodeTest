namespace TrueCodeTest.Users.Contracts.Auth;

public sealed record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime AccessExpiresAt,
    DateTime RefreshExpiresAt);
