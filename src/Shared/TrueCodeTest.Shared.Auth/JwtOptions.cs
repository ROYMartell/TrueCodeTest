namespace TrueCodeTest.Shared.Auth;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SigningKey { get; set; } = string.Empty;
    public int AccessTtlMinutes { get; set; } = 15;
    public int RefreshTtlDays { get; set; } = 7;
}
