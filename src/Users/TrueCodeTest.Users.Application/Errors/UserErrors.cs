using TrueCodeTest.Shared.Kernel;

namespace TrueCodeTest.Users.Application.Errors;

public static class UserErrors
{
    public static readonly Error NameTaken = Error.Conflict(
        "users.name_taken",
        "User with the same name already exists");

    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "users.invalid_credentials",
        "Invalid user name or password");

    public static readonly Error RefreshTokenNotFound = Error.Unauthorized(
        "users.refresh_not_found",
        "Refresh token not found");

    public static readonly Error RefreshTokenExpired = Error.Unauthorized(
        "users.refresh_expired",
        "Refresh token has expired");

    public static readonly Error RefreshTokenRevoked = Error.Unauthorized(
        "users.refresh_revoked",
        "Refresh token has been revoked");
}
