using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrueCodeTest.Shared.Auth;
using TrueCodeTest.Shared.Kernel.Abstractions;
using TrueCodeTest.Users.Application.Abstractions;
using TrueCodeTest.Users.Domain;

namespace TrueCodeTest.Users.Infrastructure.Security;

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly IDateTimeProvider _clock;
    private readonly SigningCredentials _credentials;

    public JwtTokenService(IOptions<JwtOptions> options, IDateTimeProvider clock)
    {
        _options = options.Value;
        _clock = clock;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        _credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public AccessToken CreateAccessToken(User user)
    {
        var now = _clock.UtcNow;
        var expires = now.AddMinutes(_options.AccessTtlMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Name),
            new Claim("name", user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(now).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64),
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: _credentials);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return new AccessToken(jwt, expires);
    }

    public RefreshTokenPayload CreateRefreshToken()
    {
        Span<byte> buffer = stackalloc byte[64];
        RandomNumberGenerator.Fill(buffer);
        var token = Base64UrlEncoder.Encode(buffer.ToArray());
        var hash = HashRefreshToken(token);
        var expires = _clock.UtcNow.AddDays(_options.RefreshTtlDays);
        return new RefreshTokenPayload(token, hash, expires);
    }

    public byte[] HashRefreshToken(string refreshToken) =>
        SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken));

    public TokenPair CreatePair(User user) => new(CreateAccessToken(user), CreateRefreshToken());
}
