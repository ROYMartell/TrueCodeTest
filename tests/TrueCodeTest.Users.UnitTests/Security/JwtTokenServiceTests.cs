using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TrueCodeTest.Shared.Auth;
using TrueCodeTest.Users.Domain;
using TrueCodeTest.Users.Infrastructure.Security;
using TrueCodeTest.Users.UnitTests.Common;

namespace TrueCodeTest.Users.UnitTests.Security;

public sealed class JwtTokenServiceTests
{
    private static JwtTokenService CreateService(FakeClock clock, JwtOptions? opts = null)
    {
        opts ??= new JwtOptions
        {
            Issuer = "tests",
            Audience = "tests-clients",
            SigningKey = "test-secret-key-that-is-long-enough-012345",
            AccessTtlMinutes = 15,
            RefreshTtlDays = 7,
        };
        return new JwtTokenService(Options.Create(opts), clock);
    }

    [Fact]
    public void CreateAccessToken_ProducesValidJwtWithExpectedClaims()
    {
        var clock = new FakeClock();
        var sut = CreateService(clock);
        var user = new User { Id = Guid.NewGuid(), Name = "alice" };

        var access = sut.CreateAccessToken(user);

        access.Token.Should().NotBeNullOrWhiteSpace();
        access.ExpiresAt.Should().Be(clock.UtcNow.AddMinutes(15));

        var handler = new JwtSecurityTokenHandler();
        handler.InboundClaimTypeMap.Clear();
        var principal = handler.ValidateToken(access.Token, new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "tests",
            ValidAudience = "tests-clients",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-secret-key-that-is-long-enough-012345")),
            ClockSkew = TimeSpan.Zero,
            LifetimeValidator = (notBefore, expires, _, _) =>
                expires > clock.UtcNow.AddMinutes(-1) && (notBefore is null || notBefore <= clock.UtcNow.AddSeconds(1)),
        }, out var validated);

        principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value.Should().Be(user.Id.ToString());
        principal.FindFirst("name")!.Value.Should().Be(user.Name);
        validated.Should().BeAssignableTo<JwtSecurityToken>();
    }

    [Fact]
    public void CreateRefreshToken_ProducesDeterministicHashForToken()
    {
        var clock = new FakeClock();
        var sut = CreateService(clock);

        var payload = sut.CreateRefreshToken();
        var hashAgain = sut.HashRefreshToken(payload.Token);

        payload.Token.Should().NotBeNullOrWhiteSpace();
        payload.Hash.Should().BeEquivalentTo(hashAgain);
        payload.ExpiresAt.Should().Be(clock.UtcNow.AddDays(7));
    }
}
