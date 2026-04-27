using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Application.Abstractions;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenHashAsync(byte[] tokenHash, CancellationToken ct);
    Task AddAsync(RefreshToken token, CancellationToken ct);
    void Update(RefreshToken token);
}
