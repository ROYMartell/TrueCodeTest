using TrueCodeTest.Shared.Kernel.Abstractions;

namespace TrueCodeTest.Finance.UnitTests.Common;

public sealed class FakeClock : IDateTimeProvider
{
    public DateTime UtcNow { get; set; } = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);
}
