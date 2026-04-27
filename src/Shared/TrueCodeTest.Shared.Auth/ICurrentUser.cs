namespace TrueCodeTest.Shared.Auth;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? UserName { get; }
    bool IsAuthenticated { get; }
}
