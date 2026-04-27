namespace TrueCodeTest.Finance.Domain;

public sealed class UserFavorite
{
    public Guid UserId { get; set; }
    public int CurrencyId { get; set; }
    public DateTime CreatedAt { get; set; }
}
